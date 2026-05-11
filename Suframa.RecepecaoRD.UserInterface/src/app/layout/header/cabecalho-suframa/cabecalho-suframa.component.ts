import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ROTA_LOGIN } from '../../../core/constants/constantes-app.constante';
import { AutenticacaoServico } from '../../../core/services/autenticacao.servico';
import { EstadoAutenticacaoServico } from '../../../core/services/estado-autenticacao.servico';

@Component({
  selector: 'app-cabecalho-suframa',
  templateUrl: './cabecalho-suframa.component.html',
  standalone: false,
})
export class CabecalhoSuframaComponent {
  constructor(
    private readonly autenticacaoServico: AutenticacaoServico,
    private readonly estadoAutenticacaoServico: EstadoAutenticacaoServico,
    private readonly router: Router,
  ) {}

  sair(): void {
    this.autenticacaoServico.sair().subscribe({
      next: () => {
        this.estadoAutenticacaoServico.marcarComoDesautenticado();
        this.router.navigateByUrl(`/${ROTA_LOGIN}`);
      },
      error: () => {
        this.estadoAutenticacaoServico.marcarComoDesautenticado();
        this.router.navigateByUrl(`/${ROTA_LOGIN}`);
      },
    });
  }
}
