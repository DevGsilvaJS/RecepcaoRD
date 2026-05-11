import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ROTA_LOGIN } from '../constants/constantes-app.constante';
import { Observable, Subscription, fromEvent, map, of, switchMap, take, timer } from 'rxjs';
import { AutenticacaoServico, RespostaMensagemApi } from './autenticacao.servico';
import { EstadoAutenticacaoServico } from './estado-autenticacao.servico';

@Injectable({ providedIn: 'root' })
export class RenovacaoSilenciosaServico {
  private static readonly tempoFallbackMs = 25 * 60 * 1000;

  private assinaturaEstado: Subscription | null = null;
  private assinaturaAgendamento: Subscription | null = null;

  constructor(
    private readonly autenticacaoServico: AutenticacaoServico,
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
    private readonly router: Router,
  ) {}

  iniciar(): void {
    if (this.assinaturaEstado) {
      return;
    }

    this.assinaturaEstado = this.estadoAutenticacaoServico.autenticado$.subscribe((autenticado) => {
      if (autenticado) {
        this.agendar();
      } else {
        this.cancelarAgendamento();
      }
    });
  }

  private agendar(): void {
    this.cancelarAgendamento();

    const atrasoMs = this.estadoAutenticacaoServico.obterAtrasoRenovacaoMs() ?? RenovacaoSilenciosaServico.tempoFallbackMs;

    this.assinaturaAgendamento = timer(atrasoMs)
      .pipe(switchMap(() => this.renovarQuandoVisivel()))
      .subscribe({
        next: (resposta) => {
          this.estadoAutenticacaoServico.marcarComoAutenticado(resposta?.expiraEmJwtUtc);
          if (this.estadoAutenticacaoServico.obterAutenticadoSnapshot()) {
            this.agendar();
          }
        },
        error: () => {
          this.estadoAutenticacaoServico.marcarComoDesautenticado();
          this.router.navigateByUrl(`/${ROTA_LOGIN}`);
        },
      });
  }

  private renovarQuandoVisivel(): Observable<RespostaMensagemApi | void> {
    if (!document.hidden) {
      return this.renovar();
    }

    return fromEvent(document, 'visibilitychange').pipe(
      map(() => !document.hidden),
      switchMap((visivel) => (visivel ? this.renovar() : of(undefined))),
      take(1),
    );
  }

  private renovar(): Observable<RespostaMensagemApi> {
    return this.autenticacaoServico.renovarSessao();
  }

  private cancelarAgendamento(): void {
    this.assinaturaAgendamento?.unsubscribe();
    this.assinaturaAgendamento = null;
  }
}
