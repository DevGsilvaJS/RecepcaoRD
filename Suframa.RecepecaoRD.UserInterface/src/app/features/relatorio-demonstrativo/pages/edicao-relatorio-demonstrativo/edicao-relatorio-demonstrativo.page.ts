import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { EmpresaSessao } from '../../../../core/models/empresa-sessao.modelo';
import { obterMensagemErroRespostaApi } from '../../../../core/utils/mensagem-resposta-api.util';
import { EstadoAutenticacaoServico } from '../../../../core/services/estado-autenticacao.servico';
import { NotificacaoToastServico } from '../../../../core/services/notificacao-toast.servico';
import { LinhaRelatorioDemonstrativoDto } from '../../models/relatorio-demonstrativo.modelo';
import { EdicaoRelatorioDemonstrativoServico } from '../../services/edicao-relatorio-demonstrativo.servico';
import { PlanosMemoriaServico } from '../../services/planos-memoria.servico';
import { RelatorioRdContextoEdicaoServico } from '../../services/relatorio-rd-contexto-edicao.servico';
import { RelatorioRdPlanosServico } from '../../services/relatorio-rd-planos.servico';
import {
  validarEmailCadastroRd,
  validarTelefoneCadastroRd,
} from '../../validators/cadastro-relatorio-demonstrativo.validadores';

interface OpcaoSelect<TValor extends string | number> {
  label: string;
  value: TValor;
}

interface EdicaoRelatorioDemonstrativoFormulario {
  anoBase: string;
  plano: number;
  representanteLegal: string;
  telefone: string;
  email: string;
}

@Component({
  selector: 'app-edicao-relatorio-demonstrativo-page',
  templateUrl: './edicao-relatorio-demonstrativo.page.html',
  standalone: false,
})
export class EdicaoRelatorioDemonstrativoPage implements OnInit {
  public readonly tituloPagina: string = 'Editar Relatório RD';

  public opcoesPlano: Array<OpcaoSelect<number>> = [{ label: 'Selecione o plano', value: 0 }];

  public carregandoPlanos: boolean = false;
  public salvando: boolean = false;

  public errosPorCampo: Partial<Record<keyof EdicaoRelatorioDemonstrativoFormulario, string>> = {};

  public formulario: EdicaoRelatorioDemonstrativoFormulario = {
    anoBase: '',
    plano: 0,
    representanteLegal: '',
    telefone: '',
    email: '',
  };

  public idRd: number = 0;
  public codigoRd: string = '';
  public tipoRdDescricao: string = '';

  public empresaCnpj: string = '';
  public empresaRazaoSocial: string = '';
  public empresaEndereco: string = '';
  public empresaInscricaoSuframa: string = '';

  public get empresaCnpjMascarado(): string {
    return this.aplicarMascaraCnpj(this.empresaCnpj);
  }

  public constructor(
    private readonly router: Router,
    private readonly edicaoServico: EdicaoRelatorioDemonstrativoServico,
    private readonly planosServico: RelatorioRdPlanosServico,
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
    private readonly notificacaoToastServico: NotificacaoToastServico,
    private readonly planosMemoriaServico: PlanosMemoriaServico,
    private readonly contextoEdicao: RelatorioRdContextoEdicaoServico,
    private readonly cd: ChangeDetectorRef,
  ) {}

  public ngOnInit(): void {
    this.agendarAposCicloDeDeteccao(() => {
      const linha = this.contextoEdicao.consumirLinhaParaEdicao();
      if (!linha?.id) {
        void this.router.navigateByUrl('/app/relatorio-demonstrativo');
        return;
      }

      this.aplicarLinhaNaTela(linha);
      this.sincronizarEmpresaDaSessaoSeNecessario(linha);
      this.carregarPlanosSeNecessario(linha);
      const telefoneMascarado = this.normalizarTelefoneParaMascara(linha.telefone ?? '');
      const emailValor = (linha.email ?? '').trim();
      window.setTimeout(() => {
        this.formulario.telefone = telefoneMascarado;
        this.formulario.email = emailValor;
        this.cd.markForCheck();
      }, 0);
    });
  }

  private aplicarLinhaNaTela(linha: LinhaRelatorioDemonstrativoDto): void {
    this.idRd = linha.id;
    this.codigoRd = linha.codigoRd;
    this.tipoRdDescricao = linha.tipoRd;
    this.empresaCnpj = linha.cnpj;
    this.empresaRazaoSocial = linha.razaoSocial;

    this.formulario.anoBase = String(linha.anoBase ?? '');
    this.formulario.plano = linha.planoId > 0 ? linha.planoId : 0;
    this.formulario.representanteLegal = linha.representanteLegal ?? '';
    this.formulario.email = (linha.email ?? '').trim();
    this.formulario.telefone = this.normalizarTelefoneParaMascara(linha.telefone ?? '');
  }

  private sincronizarEmpresaDaSessaoSeNecessario(linha: LinhaRelatorioDemonstrativoDto): void {
    const empresa: EmpresaSessao | null = this.estadoAutenticacaoServico.obterEmpresaSnapshot();
    if (!empresa) {
      return;
    }

    const cnpjSessao = (empresa.cnpj ?? '').replace(/\D/g, '');
    const cnpjLinha = (linha.cnpj ?? '').replace(/\D/g, '');
    if (cnpjSessao && cnpjLinha && cnpjSessao === cnpjLinha) {
      this.empresaEndereco = empresa.endereco ?? '';
      this.empresaInscricaoSuframa = empresa.inscricaoSuframa ?? '';
    }
  }

  private normalizarTelefoneParaMascara(valor: string): string {
    const digitos = (valor ?? '').replace(/\D/g, '').slice(0, 11);
    if (digitos.length === 11) {
      return digitos.replace(/^(\d{2})(\d{5})(\d{4})$/, '($1) $2-$3');
    }
    return (valor ?? '').trim();
  }

  private aplicarMascaraCnpj(cnpj: string): string {
    const somenteDigitos = (cnpj ?? '').replace(/\D/g, '');
    if (somenteDigitos.length !== 14) {
      return (cnpj ?? '').trim();
    }

    return somenteDigitos.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
  }

  private carregarPlanosSeNecessario(linha: LinhaRelatorioDemonstrativoDto): void {
    const cnpj = (this.empresaCnpj ?? '').trim();
    if (!cnpj) {
      this.garantirOpcaoPlanoDaLinha(linha, []);
      return;
    }

    const planosEmMemoria = this.planosMemoriaServico.obter(cnpj);
    if (Array.isArray(planosEmMemoria) && planosEmMemoria.length > 0) {
      this.garantirOpcaoPlanoDaLinha(linha, planosEmMemoria);
      return;
    }

    this.carregarPlanosPorCnpj(cnpj, linha);
  }

  private carregarPlanosPorCnpj(cnpj: string, linha: LinhaRelatorioDemonstrativoDto): void {
    const cnpjNormalizado = (cnpj ?? '').trim();
    if (!cnpjNormalizado) {
      this.garantirOpcaoPlanoDaLinha(linha, []);
      return;
    }

    this.carregandoPlanos = true;
    this.planosServico.listarPlanos().subscribe({
      next: (planos) => {
        const lista = Array.isArray(planos) ? planos : [];
        this.planosMemoriaServico.definir(cnpjNormalizado, lista);
        this.garantirOpcaoPlanoDaLinha(linha, lista);
        this.agendarAposCicloDeDeteccao(() => {
          this.carregandoPlanos = false;
        });
      },
      error: () => {
        this.garantirOpcaoPlanoDaLinha(linha, []);
        this.agendarAposCicloDeDeteccao(() => {
          this.carregandoPlanos = false;
        });
      },
    });
  }

  private garantirOpcaoPlanoDaLinha(
    linha: LinhaRelatorioDemonstrativoDto,
    planos: ReadonlyArray<{ id: number; numeroPlano: string }>,
  ): void {
    if (!Array.isArray(planos) || planos.length === 0) {
      if (linha.planoId > 0) {
        this.opcoesPlano = [
          { label: 'Selecione o plano', value: 0 },
          { label: linha.numeroPlano || `Plano #${linha.planoId}`, value: linha.planoId },
        ];
        this.formulario.plano = linha.planoId;
      } else {
        this.opcoesPlano = [{ label: 'Nenhum plano vinculado', value: 0 }];
        this.formulario.plano = 0;
      }
      return;
    }

    const ids = new Set(planos.map((p) => p.id));
    const opcoes: Array<OpcaoSelect<number>> = [
      { label: 'Selecione o plano', value: 0 },
      ...planos.map((p) => ({ label: p.numeroPlano, value: p.id })),
    ];

    if (linha.planoId > 0 && !ids.has(linha.planoId)) {
      opcoes.push({ label: linha.numeroPlano || `Plano #${linha.planoId}`, value: linha.planoId });
    }

    this.opcoesPlano = opcoes;
    if (linha.planoId > 0) {
      this.formulario.plano = linha.planoId;
    } else if (planos[0]) {
      this.formulario.plano = planos[0].id;
    }
  }

  public cancelar(): void {
    void this.router.navigateByUrl('/app/relatorio-demonstrativo');
  }

  public salvar(): void {
    const anoBaseNumerico = Number(this.formulario.anoBase);
    const planoIdNumerico = this.formulario.plano;

    this.errosPorCampo = {};

    if (!Number.isInteger(anoBaseNumerico) || this.formulario.anoBase.trim().length !== 4) {
      this.errosPorCampo.anoBase = 'Preencha este campo';
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

    const primeiroCampoComErro = (Object.keys(this.errosPorCampo) as Array<keyof EdicaoRelatorioDemonstrativoFormulario>)
      .find((k) => Boolean(this.errosPorCampo[k]));

    if (primeiroCampoComErro) {
      this.focarCampo(primeiroCampoComErro);
      return;
    }

    if (!this.idRd) {
      this.notificacaoToastServico.erro('Não foi possível identificar o RD para atualização.');
      return;
    }

    this.salvando = true;
    this.edicaoServico
      .atualizarRd(this.idRd, {
        anoBase: anoBaseNumerico,
        planoId: planoIdNumerico,
        representanteLegal: this.formulario.representanteLegal.trim(),
        telefone: this.formulario.telefone.trim(),
        email: this.formulario.email.trim(),
      })
      .subscribe({
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

  private focarCampo(campo: keyof EdicaoRelatorioDemonstrativoFormulario): void {
    const id = `campo-${campo}`;
    const el = document.getElementById(id);
    if (!el) return;

    const input = (el.querySelector('input, textarea, select') as HTMLElement | null) ?? el;
    if ('focus' in input) {
      (input as HTMLElement).focus();
    }
  }
}
