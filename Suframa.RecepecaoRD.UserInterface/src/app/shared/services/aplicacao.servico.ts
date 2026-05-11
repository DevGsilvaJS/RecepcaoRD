import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

/** Serviço de dados globais da aplicação (usuário, menu). Substituir por API quando disponível. */
@Injectable({ providedIn: 'root' })
export class AplicacaoServico {
  get<T = unknown>(endpoint: string): Observable<T> {
    if (endpoint === 'UsuarioLogado') {
      return of({
        usuNomeRepresentanteLogado: 'Usuário',
        usuCpfRepresentanteLogado: '000.000.000-00',
        usuNomeUsuario: 'Usuário',
        usuCpfCnpj: 'Empresa ABC',
      } as T);
    }

    if (endpoint === 'menu') {
      return of([] as T);
    }

    return of(null as T);
  }
}
