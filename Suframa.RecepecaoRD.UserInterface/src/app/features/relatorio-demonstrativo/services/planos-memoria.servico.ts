import { Injectable } from '@angular/core';

import { PlanoOpcaoDto } from '../models/cadastro-relatorio-demonstrativo.modelo';

@Injectable({ providedIn: 'root' })
export class PlanosMemoriaServico {
  private cnpj: string | null = null;
  private planos: PlanoOpcaoDto[] = [];

  public definir(cnpj: string, planos: PlanoOpcaoDto[]): void {
    this.cnpj = (cnpj ?? '').trim() || null;
    this.planos = Array.isArray(planos) ? [...planos] : [];
  }

  public limpar(): void {
    this.cnpj = null;
    this.planos = [];
  }

  public obter(cnpj: string): PlanoOpcaoDto[] | null {
    const cnpjNormalizado = (cnpj ?? '').trim();
    if (!cnpjNormalizado) {
      return null;
    }

    if (this.cnpj !== cnpjNormalizado) {
      return null;
    }

    return [...this.planos];
  }
}
