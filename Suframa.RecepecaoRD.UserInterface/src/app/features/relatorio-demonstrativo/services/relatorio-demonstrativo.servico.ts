import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { RespostaApi } from '../../../core/models/resposta-api.modelo';
import { OpcoesPaginacao, ItensPaginados } from '../../../core/models/itens-paginados.modelo';
import { ServicoApi } from '../../../core/services/servico-api.servico';
import { FiltrosRelatorioDemonstrativo, LinhaRelatorioDemonstrativoDto } from '../models/relatorio-demonstrativo.modelo';

type RdRecepcaoListaItemApi = Readonly<{
  id: number;
  numeroDeclaracaoVeracidade?: string | null;
  NumeroDeclaracaoVeracidade?: string | null;
  cnpj?: string;
  Cnpj?: string;
  razaoSocial?: string;
  RazaoSocial?: string;
  anoBase?: number;
  AnoBase?: number;
  tipoRd?: number;
  TipoRd?: number;
  planoId?: number;
  PlanoId?: number;
  numeroPlano?: string;
  NumeroPlano?: string;
  representanteLegal?: string;
  RepresentanteLegal?: string;
  telefone?: string;
  Telefone?: string;
  email?: string;
  Email?: string;
  dataCriacaoUtc?: string;
  DataCriacaoUtc?: string;
}>;

type ItensPaginadosApi<TItem> = Readonly<{
  items: TItem[];
  total: number;
}>;

type PagedItemsApi<TItem> = Readonly<{
  Items?: TItem[];
  Total?: number | null;
  items?: TItem[];
  total?: number | null;
}>;

@Injectable({ providedIn: 'root' })
export class RelatorioDemonstrativoServico {
  public constructor(private readonly servicoApi: ServicoApi) {}

  public listar(
    opcoes: OpcoesPaginacao,
    filtros: FiltrosRelatorioDemonstrativo,
  ): Observable<ItensPaginados<LinhaRelatorioDemonstrativoDto>> {
    return this.servicoApi
      .get<RespostaApi<PagedItemsApi<RdRecepcaoListaItemApi>>>('/api/relatorio-demonstrativo', {
        parametros: {
          page: opcoes.page,
          size: opcoes.size,
          sort: opcoes.sort ?? 'id',
          reverse: opcoes.reverse ?? false,
          anoBase: filtros.anoBase ?? '',
        },
      })
      .pipe(
        map((envelope): ItensPaginadosApi<RdRecepcaoListaItemApi> => {
          if (!envelope?.sucesso || !envelope.dados) {
            return { items: [], total: 0 };
          }
          const dados = envelope.dados;
          return {
            items: dados.items ?? dados.Items ?? [],
            total: Number(dados.total ?? dados.Total ?? 0),
          };
        }),
        map((paginado): ItensPaginados<LinhaRelatorioDemonstrativoDto> => ({
          total: paginado.total,
          items: paginado.items.map((rd) => this.mapearLinha(rd)),
        })),
      );
  }

  private mapearLinha(rd: RdRecepcaoListaItemApi): LinhaRelatorioDemonstrativoDto {
    const tipoNumero = Number(rd.tipoRd ?? rd.TipoRd ?? 0);
    return {
      id: rd.id,
      codigoRd: this.obterCodigoRd(rd),
      tipoRd: this.descreverTipoRd(tipoNumero),
      tipoRdNumero: tipoNumero,
      cnpj: String(rd.cnpj ?? rd.Cnpj ?? ''),
      razaoSocial: String(rd.razaoSocial ?? rd.RazaoSocial ?? ''),
      anoBase: Number(rd.anoBase ?? rd.AnoBase ?? 0),
      planoId: Number(rd.planoId ?? rd.PlanoId ?? 0),
      numeroPlano: String(rd.numeroPlano ?? rd.NumeroPlano ?? ''),
      representanteLegal: String(rd.representanteLegal ?? rd.RepresentanteLegal ?? ''),
      telefone: String(rd.telefone ?? rd.Telefone ?? ''),
      email: String(rd.email ?? rd.Email ?? ''),
      dataCriacaoUtc: String(rd.dataCriacaoUtc ?? rd.DataCriacaoUtc ?? ''),
    };
  }

  private obterCodigoRd(rd: RdRecepcaoListaItemApi): string {
    const codigo = (rd.numeroDeclaracaoVeracidade ?? rd.NumeroDeclaracaoVeracidade ?? '').trim();
    return codigo ? codigo : '--';
  }

  private descreverTipoRd(tipoRd: number): string {
    switch (tipoRd) {
      case 1:
        return 'ORIGINAL';
      case 2:
        return 'RETIFICAÇÃO RD ORIGINAL';
      case 3:
        return 'CONTESTAÇÃO';
      case 4:
        return 'RETIFICAÇÃO RD CONTESTAÇÃO';
      case 5:
        return 'RECURSO';
      case 6:
        return 'RETIFICAÇÃO RD RECURSO';
      default:
        return String(tipoRd);
    }
  }
}
