import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { EstadoAutenticacaoServico } from '../../../core/services/estado-autenticacao.servico';
import { OpcoesPaginacao, ItensPaginados } from '../../../core/models/itens-paginados.modelo';
import { FiltrosRelatorioDemonstrativo, LinhaRelatorioDemonstrativoDto } from '../models/relatorio-demonstrativo.modelo';
import { PlanosMemoriaServico } from '../services/planos-memoria.servico';
import { RelatorioDemonstrativoServico } from '../services/relatorio-demonstrativo.servico';
import { RelatorioRdPlanosServico } from '../services/relatorio-rd-planos.servico';

/**
 * Orquestra listagem e pré-carga de dados auxiliares da feature Relatório Demonstrativo.
 */
@Injectable({ providedIn: 'root' })
export class RelatorioDemonstrativoFacade {
  public constructor(
    private readonly relatorioDemonstrativoServico: RelatorioDemonstrativoServico,
    private readonly relatorioRdPlanosServico: RelatorioRdPlanosServico,
    private readonly planosMemoriaServico: PlanosMemoriaServico,
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
  ) {}

  public listar(
    opcoes: OpcoesPaginacao,
    filtros: FiltrosRelatorioDemonstrativo,
  ): Observable<ItensPaginados<LinhaRelatorioDemonstrativoDto>> {
    return this.relatorioDemonstrativoServico.listar(opcoes, filtros);
  }

  public preCarregarPlanosDaEmpresa(): void {
    const empresa = this.estadoAutenticacaoServico.obterEmpresaSnapshot();
    const cnpj = (empresa?.cnpj ?? '').trim();
    if (!cnpj) {
      return;
    }

    this.relatorioRdPlanosServico.listarPlanos().subscribe({
      next: (planos) => this.planosMemoriaServico.definir(cnpj, planos ?? []),
      error: () => this.planosMemoriaServico.limpar(),
    });
  }
}
