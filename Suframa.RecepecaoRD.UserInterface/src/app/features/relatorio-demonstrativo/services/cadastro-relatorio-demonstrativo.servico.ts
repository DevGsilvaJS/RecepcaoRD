import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { RespostaApi } from '../../../core/models/resposta-api.modelo';
import { ServicoApi } from '../../../core/services/servico-api.servico';
import { CriarRdRecepcaoDto } from '../models/cadastro-relatorio-demonstrativo.modelo';

/**
 * Apenas criação (POST) de novo RD.
 */
@Injectable({ providedIn: 'root' })
export class CadastroRelatorioDemonstrativoServico {
  public constructor(private readonly servicoApi: ServicoApi) {}

  public criarRd(dto: CriarRdRecepcaoDto): Observable<RespostaApi<null>> {
    return this.servicoApi.post<RespostaApi<null>, CriarRdRecepcaoDto>('/api/relatorio-demonstrativo', dto);
  }
}
