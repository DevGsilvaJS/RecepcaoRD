import { Injectable } from '@angular/core';

import { LinhaRelatorioDemonstrativoDto } from '../models/relatorio-demonstrativo.modelo';

/**
 * Transporta a linha selecionada na grade para a página de edição (sem GET adicional).
 */
@Injectable({ providedIn: 'root' })
export class RelatorioRdContextoEdicaoServico {
  private linha: LinhaRelatorioDemonstrativoDto | null = null;

  public definirLinhaParaEdicao(linha: LinhaRelatorioDemonstrativoDto): void {
    this.linha = linha;
  }

  public consumirLinhaParaEdicao(): LinhaRelatorioDemonstrativoDto | null {
    const atual = this.linha;
    this.linha = null;
    return atual;
  }

  public limpar(): void {
    this.linha = null;
  }
}
