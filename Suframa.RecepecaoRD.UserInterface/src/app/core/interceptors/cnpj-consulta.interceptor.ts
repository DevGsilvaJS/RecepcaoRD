import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { EstadoAutenticacaoServico } from '../services/estado-autenticacao.servico';

/**
 * Anexa o CNPJ da empresa em sessão (sessionStorage) às requisições GET para a API,
 * exceto rotas de autenticação.
 */
@Injectable()
export class CnpjConsultaInterceptor implements HttpInterceptor {
  public constructor(private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico) {}

  public intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (req.method !== 'GET') {
      return next.handle(req);
    }

    if (!this.deveAnexarCnpj(req)) {
      return next.handle(req);
    }

    const cnpjSomenteDigitos = this.obterCnpjEmpresaSomenteDigitos();
    if (!cnpjSomenteDigitos) {
      return next.handle(req);
    }

    if (req.params.has('cnpj')) {
      return next.handle(req);
    }

    return next.handle(
      req.clone({
        setParams: { cnpj: cnpjSomenteDigitos },
      }),
    );
  }

  private deveAnexarCnpj(req: HttpRequest<unknown>): boolean {
    const url = req.url;
    if (!url.includes('/api/')) {
      return false;
    }

    if (url.includes('/api/autenticacao')) {
      return false;
    }

    return true;
  }

  private obterCnpjEmpresaSomenteDigitos(): string {
    const bruto = this.estadoAutenticacaoServico.obterEmpresaSnapshot()?.cnpj ?? '';
    return (bruto ?? '').replace(/\D/g, '').trim();
  }
}
