import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { RespostaApi } from '../../../core/models/resposta-api.modelo';
import { ServicoApi } from '../../../core/services/servico-api.servico';
import { PlanoOpcaoDto } from '../models/cadastro-relatorio-demonstrativo.modelo';

type PlanoListaItemApi = Readonly<{
  id: number;
  numeroPlano: string;
  Id?: number;
  NumeroPlano?: string;
}>;

/**
 * Apenas listagem de planos de recepção (CNPJ via interceptor).
 */
@Injectable({ providedIn: 'root' })
export class RelatorioRdPlanosServico {
  public constructor(private readonly servicoApi: ServicoApi) {}

  public listarPlanos(): Observable<PlanoOpcaoDto[]> {
    return this.servicoApi.get<RespostaApi<PlanoListaItemApi[]>>('/api/relatorio-demonstrativo/planos').pipe(
      map((envelope) => {
        if (!envelope?.sucesso || !Array.isArray(envelope.dados)) {
          return [];
        }
        return envelope.dados.map((p) => ({
          id: Number(p.id ?? p.Id ?? 0),
          numeroPlano: String(p.numeroPlano ?? p.NumeroPlano ?? ''),
        }));
      }),
    );
  }
}
