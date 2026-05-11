import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { AutenticacaoServico, RespostaLoginCompletaApi } from '../../../core/services/autenticacao.servico';
import { EstadoAutenticacaoServico } from '../../../core/services/estado-autenticacao.servico';

@Injectable({ providedIn: 'root' })
export class AutenticacaoFacade {
  constructor(
    private readonly autenticacaoServico: AutenticacaoServico,
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
  ) {}

  /**
   * Autentica o usuário, atualiza o estado da sessão e persiste os dados da empresa no sessionStorage.
   */
  entrar(documento: string): Observable<RespostaLoginCompletaApi> {
    return this.autenticacaoServico.autenticar(documento).pipe(
      tap((resposta) => {
        this.estadoAutenticacaoServico.marcarComoAutenticado(
          resposta.expiraEmJwtUtc,
          resposta.empresa ?? null,
        );
      }),
    );
  }
}
