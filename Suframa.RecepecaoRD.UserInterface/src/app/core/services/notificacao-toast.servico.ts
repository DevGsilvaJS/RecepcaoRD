import { Injectable } from '@angular/core';
import { HotToastService } from '@ngxpert/hot-toast';

export type OpcoesToast = Readonly<{
  duracaoMs?: number;
}>;

@Injectable({ providedIn: 'root' })
export class NotificacaoToastServico {
  private static readonly duracaoPadraoMs: number = 4000;

  public constructor(private readonly toast: HotToastService) {}

  public sucesso(mensagem: string, opcoes?: OpcoesToast): void {
    window.setTimeout(() => this.toast.success(mensagem, this.montarOpcoes(opcoes)), 0);
  }

  public erro(mensagem: string, opcoes?: OpcoesToast): void {
    window.setTimeout(() => this.toast.error(mensagem, this.montarOpcoes(opcoes)), 0);
  }

  public aviso(mensagem: string, opcoes?: OpcoesToast): void {
    window.setTimeout(() => this.toast.warning(mensagem, this.montarOpcoes(opcoes)), 0);
  }

  public info(mensagem: string, opcoes?: OpcoesToast): void {
    window.setTimeout(() => this.toast.info(mensagem, this.montarOpcoes(opcoes)), 0);
  }

  private montarOpcoes(opcoes?: OpcoesToast): { duration: number; autoClose: boolean } {
    const duracao = opcoes?.duracaoMs ?? NotificacaoToastServico.duracaoPadraoMs;
    return { duration: duracao, autoClose: true };
  }
}
