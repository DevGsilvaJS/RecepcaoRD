import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, finalize, map, shareReplay, switchMap, throwError } from 'rxjs';
import { ROTA_LOGIN } from '../constants/constantes-app.constante';
import { AutenticacaoServico } from '../services/autenticacao.servico';
import { EstadoAutenticacaoServico } from '../services/estado-autenticacao.servico';

@Injectable()
export class RenovacaoSessaoInterceptor implements HttpInterceptor {
  private renovacaoEmAndamento$: Observable<void> | null = null;

  constructor(
    private readonly autenticacaoServico: AutenticacaoServico,
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
    private readonly router: Router,
  ) {}

  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(req).pipe(
      catchError((erro: unknown) => {
        if (!(erro instanceof HttpErrorResponse) || erro.status !== 401) {
          return throwError(() => erro);
        }

        if (this.ehRequisicaoDeAutenticacao(req)) {
          this.estadoAutenticacaoServico.marcarComoDesautenticado();
          this.router.navigateByUrl(`/${ROTA_LOGIN}`);
          return throwError(() => erro);
        }

        return this.obterOuIniciarRenovacao().pipe(
          switchMap(() => next.handle(req)),
          catchError((erroAposRenovar: unknown) => {
            if (erroAposRenovar instanceof HttpErrorResponse && erroAposRenovar.status === 401) {
              this.estadoAutenticacaoServico.marcarComoDesautenticado();
              this.router.navigateByUrl(`/${ROTA_LOGIN}`);
            }
            return throwError(() => erroAposRenovar);
          }),
        );
      }),
    );
  }

  private obterOuIniciarRenovacao(): Observable<void> {
    if (!this.renovacaoEmAndamento$) {
      this.renovacaoEmAndamento$ = this.autenticacaoServico.renovarSessao().pipe(
        map((resposta) => {
          this.estadoAutenticacaoServico.marcarComoAutenticado(resposta?.expiraEmJwtUtc);
          return undefined;
        }),
        finalize(() => {
          this.renovacaoEmAndamento$ = null;
        }),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    }

    return this.renovacaoEmAndamento$;
  }

  private ehRequisicaoDeAutenticacao(req: HttpRequest<unknown>): boolean {
    const url = req.url;
    return (
      url.includes('/api/autenticacao/refresh') ||
      url.includes('/api/autenticacao/logout')
    );
  }
}
