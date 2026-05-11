import { Component, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AutenticacaoFacade } from '../facade/autenticacao.facade';

@Component({
  selector: 'app-login-page',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.css'],
  standalone: false,
})
export class LoginPage {
  protected readonly carregando = signal(false);
  protected readonly mensagem = signal('');
  protected readonly documentoDigitado = signal('');

  protected readonly documentoSomenteDigitos = computed(() =>
    (this.documentoDigitado() ?? '').replace(/\D/g, '').trim(),
  );

  constructor(
    private readonly autenticacaoFacade: AutenticacaoFacade,
    private readonly router: Router,
  ) {}

  entrar(): void {
    if (this.carregando()) {
      return;
    }

    const documentoSomenteDigitos = this.documentoSomenteDigitos();
    if (!documentoSomenteDigitos) {
      this.mensagem.set('Informe o CPF ou o CNPJ para autenticar.');
      return;
    }

    const tamanho = documentoSomenteDigitos.length;
    if (tamanho !== 11 && tamanho !== 14) {
      this.mensagem.set('Documento inválido (informe 11 ou 14 dígitos).');
      return;
    }

    this.carregando.set(true);
    this.mensagem.set('');

    this.autenticacaoFacade.entrar(documentoSomenteDigitos).subscribe({
      next: (resposta) => {
        this.mensagem.set(resposta.mensagem ?? 'Autenticado.');
        this.carregando.set(false);
        this.router.navigateByUrl('/app/relatorio-demonstrativo');
      },
      error: (erro: unknown) => {
        const texto = erro instanceof Error ? erro.message : 'Falha ao autenticar.';
        this.mensagem.set(texto);
        this.carregando.set(false);
      },
    });
  }
}
