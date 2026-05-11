import { Injectable } from '@angular/core';
import { CanActivateChild, Router, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { ROTA_LOGIN } from '../constants/constantes-app.constante';
import { EstadoAutenticacaoServico } from '../services/estado-autenticacao.servico';

@Injectable({ providedIn: 'root' })
export class AutenticacaoGuard implements CanActivateChild {
  constructor(
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
    private readonly router: Router,
  ) {}

  canActivateChild(): Observable<boolean | UrlTree> {
    return this.estadoAutenticacaoServico.autenticado$.pipe(
      map((autenticado) => (autenticado ? true : this.router.parseUrl(`/${ROTA_LOGIN}`))),
    );
  }
}
