import { ChangeDetectorRef, Component, NgZone, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { OpcoesPaginacao } from '../../../../core/models/itens-paginados.modelo';
import { ColunaGridDados } from '../../../../shared/interfaces/coluna-grid-dados.interface';
import { RelatorioDemonstrativoFacade } from '../../facade/relatorio-demonstrativo.facade';
import { LinhaRelatorioDemonstrativoDto } from '../../models/relatorio-demonstrativo.modelo';
import { RelatorioRdContextoEdicaoServico } from '../../services/relatorio-rd-contexto-edicao.servico';

@Component({
  selector: 'app-listagem-relatorio-demonstrativo-page',
  templateUrl: './listagem-relatorio-demonstrativo.page.html',
  standalone: false,
})
export class ListagemRelatorioDemonstrativoPage implements OnInit {
  public readonly tituloGrid: string = 'Empresa Relatório Demonstrativo';
  public get subtituloGrid(): string {
    return `(${this.total} registros)`;
  }

  private readonly anoAtual: number = new Date().getFullYear();

  public readonly anosBase: Array<{ label: string; value: number }> = [
    { label: String(this.anoAtual), value: this.anoAtual },
    { label: String(this.anoAtual - 1), value: this.anoAtual - 1 },
    { label: String(this.anoAtual - 2), value: this.anoAtual - 2 },
  ];

  public anoBaseSelecionado: number = this.anoAtual;

  public readonly colunas: ColunaGridDados[] = [
    { campo: 'codigoRd', cabecalho: 'Código RD', ordenavel: true },
    { campo: 'tipoRd', cabecalho: 'Tipo RD', ordenavel: true },
    { campo: 'acoes', cabecalho: 'Opções', alinharDireita: true, largura: '170px' },
  ];

  public dados: LinhaRelatorioDemonstrativoDto[] = [];
  public total: number = 0;
  public page: number = 1;
  public size: number = 10;
  public readonly arrayPageSizeOptions: number[] = [10, 20, 50];
  private versaoRequisicaoListar: number = 0;

  public readonly parametros: OpcoesPaginacao = {
    page: 1,
    size: 10,
    sort: 'codigoRd',
    reverse: false,
  };

  public constructor(
    private readonly relatorioDemonstrativoFacade: RelatorioDemonstrativoFacade,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly zona: NgZone,
    private readonly cd: ChangeDetectorRef,
    private readonly contextoEdicaoRd: RelatorioRdContextoEdicaoServico,
  ) {
    this.observarRecargaAoVoltarDoCadastro();
  }

  public ngOnInit(): void {
    this.parametros.page = 1;
    this.page = 1;
    this.anoBaseSelecionado = this.anoAtual;
    this.preCarregarPlanosDaEmpresa();
    this.listar();
  }

  private preCarregarPlanosDaEmpresa(): void {
    this.relatorioDemonstrativoFacade.preCarregarPlanosDaEmpresa();
  }

  public aoMudarPagina(pagina: number): void {
    this.page = pagina;
    this.parametros.page = pagina;
    this.listar();
  }

  public aoMudarTamanhoPagina(tamanho: number): void {
    this.size = tamanho;
    this.parametros.size = tamanho;
    this.parametros.page = 1;
    this.page = 1;
    this.listar();
  }

  public aoMudarAnoBase(ano: number): void {
    this.anoBaseSelecionado = ano;
    this.parametros.page = 1;
    this.page = 1;
    this.listar();
  }

  private listar(): void {
    const versao = ++this.versaoRequisicaoListar;
    this.relatorioDemonstrativoFacade
      .listar(this.parametros, {
        anoBase: this.anoBaseSelecionado,
        situacaoAtual: 'RD Aguardando Análise',
      })
      .subscribe((resultado) => {
        if (versao !== this.versaoRequisicaoListar) {
          return;
        }

        setTimeout(() => {
          if (versao !== this.versaoRequisicaoListar) {
            return;
          }

          this.zona.run(() => {
            this.dados = [...resultado.items];
            this.total = resultado.total;
          });

          this.cd.detectChanges();
        }, 0);
      });
  }

  private observarRecargaAoVoltarDoCadastro(): void {
    this.route.queryParamMap.subscribe((params) => {
      const recarregar = params.get('recarregar') === '1';
      if (!recarregar) {
        return;
      }

      this.listar();

      void this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { recarregar: null },
        queryParamsHandling: 'merge',
        replaceUrl: true,
      });
    });
  }

  public novo(): void {
    void this.router.navigateByUrl('/app/relatorio-demonstrativo/cadastro-relatorio-demonstrativo');
  }

  public exportar(): void {
    // TODO: integrar com exportação real quando disponível
  }

  public editar(linha: LinhaRelatorioDemonstrativoDto): void {
    this.contextoEdicaoRd.definirLinhaParaEdicao(linha);
    void this.router.navigateByUrl('/app/relatorio-demonstrativo/edicao-relatorio-demonstrativo');
  }

  public excluir(linha: LinhaRelatorioDemonstrativoDto): void {
    void linha;
  }

  public copiar(linha: LinhaRelatorioDemonstrativoDto): void {
    void linha;
  }

  public abrirDetalhes(linha: LinhaRelatorioDemonstrativoDto): void {
    void linha;
  }
}
