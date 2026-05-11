import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { RespostaApi } from '../../../core/models/resposta-api.modelo';
import { ServicoApi } from '../../../core/services/servico-api.servico';
import { AtualizarRdRecepcaoDto } from '../models/cadastro-relatorio-demonstrativo.modelo';

/**
 * Apenas atualização (PUT) de RD existente.
 */
@Injectable({ providedIn: 'root' })
export class EdicaoRelatorioDemonstrativoServico {
  public constructor(private readonly servicoApi: ServicoApi) {}

  public atualizarRd(id: number, dto: AtualizarRdRecepcaoDto): Observable<RespostaApi<null>> {
    return this.servicoApi.put<RespostaApi<null>, AtualizarRdRecepcaoDto>(`/api/relatorio-demonstrativo/${id}`, dto);
  }
}
