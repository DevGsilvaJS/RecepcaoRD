import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export type OpcoesRequisicaoApi = Readonly<{
  parametros?: Readonly<Record<string, string | number | boolean>>;
  cabecalhos?: Readonly<Record<string, string>>;
}>;

@Injectable({ providedIn: 'root' })
export class ServicoApi {
  private readonly urlBaseApi: string = this.normalizarUrlBase(environment.serviceUrl);

  constructor(private readonly http: HttpClient) {}

  get<TResposta>(caminho: string, opcoes?: OpcoesRequisicaoApi): Observable<TResposta> {
    return this.http.get<TResposta>(this.montarUrl(caminho), this.montarOpcoes(opcoes));
  }

  post<TResposta, TCorpo>(caminho: string, corpo: TCorpo, opcoes?: OpcoesRequisicaoApi): Observable<TResposta> {
    return this.http.post<TResposta>(this.montarUrl(caminho), corpo, this.montarOpcoes(opcoes));
  }

  put<TResposta, TCorpo>(caminho: string, corpo: TCorpo, opcoes?: OpcoesRequisicaoApi): Observable<TResposta> {
    return this.http.put<TResposta>(this.montarUrl(caminho), corpo, this.montarOpcoes(opcoes));
  }

  delete<TResposta>(caminho: string, opcoes?: OpcoesRequisicaoApi): Observable<TResposta> {
    return this.http.delete<TResposta>(this.montarUrl(caminho), this.montarOpcoes(opcoes));
  }

  private montarUrl(caminho: string): string {
    const base = this.urlBaseApi ?? '';
    if (!base) {
      return caminho.startsWith('/') ? caminho : `/${caminho}`;
    }

    const baseSemBarraFinal = base.endsWith('/') ? base.slice(0, -1) : base;
    const caminhoComBarra = caminho.startsWith('/') ? caminho : `/${caminho}`;
    return `${baseSemBarraFinal}${caminhoComBarra}`;
  }

  private normalizarUrlBase(url: string): string {
    return (url ?? '').trim();
  }

  private montarOpcoes(opcoes?: OpcoesRequisicaoApi): { headers?: HttpHeaders; params?: HttpParams } {
    const headers = opcoes?.cabecalhos ? new HttpHeaders(opcoes.cabecalhos) : undefined;

    const params = opcoes?.parametros
      ? Object.entries(opcoes.parametros).reduce((p, [chave, valor]) => p.set(chave, String(valor)), new HttpParams())
      : undefined;

    return { headers, params };
  }
}
