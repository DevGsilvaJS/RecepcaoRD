import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, take } from 'rxjs';

import { EmpresaSessao } from '../../../../core/models/empresa-sessao.modelo';
import { obterMensagemErroRespostaApi } from '../../../../core/utils/mensagem-resposta-api.util';
import { EstadoAutenticacaoServico } from '../../../../core/services/estado-autenticacao.servico';
import { NotificacaoToastServico } from '../../../../core/services/notificacao-toast.servico';
import { CadastroRelatorioDemonstrativoServico } from '../../services/cadastro-relatorio-demonstrativo.servico';
import { PlanosMemoriaServico } from '../../services/planos-memoria.servico';
import { RelatorioRdPlanosServico } from '../../services/relatorio-rd-planos.servico';
import {
  validarEmailCadastroRd,
  validarTelefoneCadastroRd,
} from '../../validators/cadastro-relatorio-demonstrativo.validadores';

interface OpcaoSelect<TValor extends string | number> {
  label: string;
  value: TValor;
}

interface CadastroRelatorioDemonstrativoFormulario {
  anoBase: string;
  tipoRd: number;
  plano: number;
  representanteLegal: string;
  telefone: string;
  email: string;
}

@Component({
  selector: 'app-cadastro-relatorio-demonstrativo-page',
  templateUrl: './cadastro-relatorio-demonstrativo.page.html',
  standalone: false,
})
export class CadastroRelatorioDemonstrativoPage implements OnInit {
  public readonly tituloPagina: string = 'Novo Relatório RD';

  public opcoesTipoRd: Array<OpcaoSelect<number>> = [
    { label: 'ORIGINAL', value: 1 },
    { label: 'RETIFICAÇÃO RD ORIGINAL', value: 2 },
    { label: 'CONTESTAÇÃO', value: 3 },
    { label: 'RETIFICAÇÃO RD CONTESTAÇÃO', value: 4 },
    { label: 'RECURSO', value: 5 },
    { label: 'RETIFICAÇÃO RD RECURSO', value: 6 },
  ];

  public opcoesPlano: Array<OpcaoSelect<number>> = [{ label: 'Selecione o plano', value: 0 }];

  public carregandoPlanos: boolean = false;
  public salvando: boolean = false;

  public errosPorCampo: Partial<Record<keyof CadastroRelatorioDemonstrativoFormulario, string>> = {};

  public formulario: CadastroRelatorioDemonstrativoFormulario = {
    anoBase: '',
    tipoRd: 1,
    plano: 0,
    representanteLegal: '',
    telefone: '',
    email: '',
  };

  public empresaCnpj: string = '';
  public empresaRazaoSocial: string = '';
  public empresaEndereco: string = '';
  public empresaInscricaoSuframa: string = '';

  public get empresaCnpjMascarado(): string {
    return this.aplicarMascaraCnpj(this.empresaCnpj);
  }

  public constructor(
    private readonly router: Router,
    private readonly cadastroServico: CadastroRelatorioDemonstrativoServico,
    private readonly planosServico: RelatorioRdPlanosServico,
    private readonly route: ActivatedRoute,
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
    private readonly notificacaoToastServico: NotificacaoToastServico,
    private readonly planosMemoriaServico: PlanosMemoriaServico,
    private readonly cd: ChangeDetectorRef,
  ) {
    this.definirEmpresaDaSessao();
    this.observarEmpresaDaSessao();
    this.carregarPlanosSeNecessario();
  }

  public ngOnInit(): void {
    this.definirTipoRdInicial();
  }

  private definirEmpresaDaSessao(): void {
    const empresa: EmpresaSessao | null = this.estadoAutenticacaoServico.obterEmpresaSnapshot();
    if (!empresa) {
      return;
    }

    this.preencherEmpresa(empresa);
  }

  private observarEmpresaDaSessao(): void {
    this.estadoAutenticacaoServico.empresa$
      .pipe(
        filter((e): e is EmpresaSessao => Boolean(e?.cnpj?.trim())),
        take(1),
      )
      .subscribe((empresa) => this.preencherEmpresa(empresa));
  }

  private preencherEmpresa(empresa: EmpresaSessao): void {
    this.empresaCnpj = empresa.cnpj ?? '';
    this.empresaRazaoSocial = empresa.razaoSocial ?? '';
    this.empresaEndereco = empresa.endereco ?? '';
    this.empresaInscricaoSuframa = empresa.inscricaoSuframa ?? '';
  }

  private aplicarMascaraCnpj(cnpj: string): string {
    const somenteDigitos = (cnpj ?? '').replace(/\D/g, '');
    if (somenteDigitos.length !== 14) {
      return (cnpj ?? '').trim();
    }

    return somenteDigitos.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
  }

  private carregarPlanosSeNecessario(): void {
    const cnpj = (this.empresaCnpj ?? '').trim();
    if (!cnpj) {
      return;
    }

    const planosEmMemoria = this.planosMemoriaServico.obter(cnpj);
    if (Array.isArray(planosEmMemoria) && planosEmMemoria.length > 0) {
      this.definirOpcoesPlano(planosEmMemoria);
      return;
    }

    this.carregarPlanosPorCnpj(cnpj);
  }

  private carregarPlanosPorCnpj(cnpj: string): void {
    const cnpjNormalizado = (cnpj ?? '').trim();
    if (!cnpjNormalizado) {
      this.opcoesPlano = [{ label: 'Nenhum plano vinculado', value: 0 }];
      this.formulario.plano = 0;
      return;
    }

    this.carregandoPlanos = true;
    this.planosServico.listarPlanos().subscribe({
      next: (planos) => {
        const lista = Array.isArray(planos) ? planos : [];
        this.planosMemoriaServico.definir(cnpjNormalizado, lista);
        this.definirOpcoesPlano(lista);
        this.agendarAposCicloDeDeteccao(() => {
          this.carregandoPlanos = false;
        });
      },
      error: () => {
        this.opcoesPlano = [{ label: 'Nenhum plano vinculado', value: 0 }];
        this.formulario.plano = 0;
        this.agendarAposCicloDeDeteccao(() => {
          this.carregandoPlanos = false;
        });
      },
    });
  }

  private definirOpcoesPlano(planos: ReadonlyArray<{ id: number; numeroPlano: string }>): void {
    if (!Array.isArray(planos) || planos.length === 0) {
      this.opcoesPlano = [{ label: 'Nenhum plano vinculado', value: 0 }];
      this.formulario.plano = 0;
      return;
    }

    this.opcoesPlano = [
      { label: 'Selecione o plano', value: 0 },
      ...planos.map((p) => ({ label: p.numeroPlano, value: p.id })),
    ];

    if (this.formulario.plano <= 0) {
      this.formulario.plano = planos[0].id;
    }
  }

  public cancelar(): void {
    void this.router.navigateByUrl('/app/relatorio-demonstrativo');
  }

  public salvar(): void {
    const anoBaseNumerico = Number(this.formulario.anoBase);
    const tipoRdNumerico = this.formulario.tipoRd;
    const planoIdNumerico = this.formulario.plano;

    this.errosPorCampo = {};

    if (!Number.isInteger(anoBaseNumerico) || this.formulario.anoBase.trim().length !== 4) {
      this.errosPorCampo.anoBase = 'Preencha este campo';
    }

    if (!Number.isInteger(tipoRdNumerico) || tipoRdNumerico < 1 || tipoRdNumerico > 6) {
      this.errosPorCampo.tipoRd = 'Preencha este campo';
    }

    if (!Number.isInteger(planoIdNumerico) || planoIdNumerico <= 0) {
      this.errosPorCampo.plano = 'Preencha este campo';
    }

    if (!this.formulario.representanteLegal.trim()) {
      this.errosPorCampo.representanteLegal = 'Preencha este campo';
    }

    if (this.formulario.representanteLegal.trim().length > 155) {
      this.errosPorCampo.representanteLegal = 'Preencha este campo';
    }

    if (!validarTelefoneCadastroRd(this.formulario.telefone)) {
      this.errosPorCampo.telefone = 'Preencha este campo';
    }

    if (!this.formulario.email.trim()) {
      this.errosPorCampo.email = 'Preencha este campo';
    } else if (!validarEmailCadastroRd(this.formulario.email)) {
      this.errosPorCampo.email = 'E-mail inválido';
    }

    const primeiroCampoComErro = (Object.keys(this.errosPorCampo) as Array<keyof CadastroRelatorioDemonstrativoFormulario>)
      .find((k) => Boolean(this.errosPorCampo[k]));

    if (primeiroCampoComErro) {
      this.focarCampo(primeiroCampoComErro);
      return;
    }

    this.salvando = true;
    const idOrigemParam = this.route.snapshot.queryParamMap.get('idOrigem');
    const idOrigem = idOrigemParam ? Number(idOrigemParam) : null;

    this.cadastroServico.criarRd({
      cnpj: this.empresaCnpj,
      razaoSocial: this.empresaRazaoSocial,
      endereco: this.empresaEndereco,
      inscricaoSuframa: this.empresaInscricaoSuframa,
      anoBase: anoBaseNumerico,
      tipoRd: tipoRdNumerico,
      planoId: planoIdNumerico,
      representanteLegal: this.formulario.representanteLegal.trim(),
      telefone: this.formulario.telefone.trim(),
      email: this.formulario.email.trim(),
      idOrigem: Number.isFinite(idOrigem) ? idOrigem : null,
    }).subscribe({
      next: (resposta) => {
        this.salvando = false;
        if (resposta.sucesso) {
          this.notificacaoToastServico.sucesso((resposta.mensagem ?? '').trim() || 'Gravado com sucesso.');
          void this.router.navigateByUrl('/app/relatorio-demonstrativo?recarregar=1');
        } else {
          this.notificacaoToastServico.erro(obterMensagemErroRespostaApi(resposta));
        }
        this.cd.markForCheck();
      },
      error: (e: unknown) => {
        console.error(e);
        this.salvando = false;
        this.notificacaoToastServico.erro('Não foi possível comunicar com o servidor.');
        this.cd.markForCheck();
      },
    });
  }

  private agendarAposCicloDeDeteccao(acao: () => void): void {
    window.setTimeout(() => {
      acao();
      this.cd.markForCheck();
    }, 0);
  }

  private focarCampo(campo: keyof CadastroRelatorioDemonstrativoFormulario): void {
    const id = `campo-${campo}`;
    const el = document.getElementById(id);
    if (!el) return;

    const input = (el.querySelector('input, textarea, select') as HTMLElement | null) ?? el;
    if ('focus' in input) {
      (input as HTMLElement).focus();
    }
  }

  private definirTipoRdInicial(): void {
    const tipo = Number(this.route.snapshot.queryParamMap.get('tipoRd'));
    if (Number.isInteger(tipo) && tipo >= 1 && tipo <= 6) {
      this.formulario.tipoRd = tipo;
    }
  }
}
