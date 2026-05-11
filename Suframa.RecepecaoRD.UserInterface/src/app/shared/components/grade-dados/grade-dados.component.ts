import { Component, ContentChildren, EventEmitter, Input, Output, QueryList } from '@angular/core';

import { NomeTemplateGrid, TemplateGridDirective } from '../../directives/template-grade-dados.directive';
import { ColunaGridDados } from '../../interfaces/coluna-grid-dados.interface';

@Component({
  selector: 'app-grid',
  templateUrl: './grade-dados.component.html',
  styleUrls: ['./grade-dados.component.css'],
  standalone: false,
})
export class GridComponent {
  @Input({ required: true })
  public titulo!: string;

  @Input()
  public subtitulo?: string;

  @Input({ required: true })
  public colunas!: ColunaGridDados[];

  @Input({ required: true })
  public dados!: unknown[];

  @Input()
  public carregando: boolean = false;

  @Input()
  public page: number = 1;

  @Input()
  public size: number = 10;

  @Input()
  public total: number = 0;

  @Input()
  public arrayPageSizeOptions: number[] = [10, 20, 50];

  @Output()
  public readonly onChangePage: EventEmitter<number> = new EventEmitter<number>();

  @Output()
  public readonly onChangeSize: EventEmitter<number> = new EventEmitter<number>();

  @ContentChildren(TemplateGridDirective)
  private readonly templates?: QueryList<TemplateGridDirective>;

  public obterTemplate(nome: NomeTemplateGrid): TemplateGridDirective | undefined {
    return this.templates?.find((t: TemplateGridDirective) => t.nomeTemplate === nome);
  }

  public obterPrimeiraLinha(): number {
    return Math.max(0, (this.page - 1) * this.size);
  }

  public obterTextoIntervalo(): string {
    if (this.total <= 0) {
      return '0–0 de 0';
    }
    const inicio = this.obterPrimeiraLinha() + 1;
    const fim = Math.min(this.total, this.obterPrimeiraLinha() + this.size);
    return `${inicio}–${fim} de ${this.total}`;
  }

  public obterTotalPaginas(): number {
    if (this.total <= 0 || this.size <= 0) {
      return 1;
    }
    return Math.max(1, Math.ceil(this.total / this.size));
  }

  public obterTextoPaginaAtual(): string {
    return `Página ${this.page} de ${this.obterTotalPaginas()}`;
  }

  public aoMudarTamanhoPagina(novoTamanho: number): void {
    this.size = novoTamanho;
    this.onChangeSize.emit(novoTamanho);
    this.onChangePage.emit(1);
  }

  public aoMudarPagina(evento: { first?: number; rows?: number }): void {
    const first = evento.first ?? 0;
    const rows = evento.rows ?? this.size;
    const novaPagina = Math.floor(first / rows) + 1;
    if (rows !== this.size) {
      this.size = rows;
      this.onChangeSize.emit(rows);
    }
    this.page = novaPagina;
    this.onChangePage.emit(novaPagina);
  }

  public obterValor(linha: unknown, campo: string): unknown {
    if (linha && typeof linha === 'object' && campo in (linha as Record<string, unknown>)) {
      return (linha as Record<string, unknown>)[campo];
    }
    return undefined;
  }

  public obterOpcoesRegistrosPorPagina(opcoes: number[]): Array<{ label: string; value: number }> {
    return opcoes.map((v: number) => ({ label: String(v), value: v }));
  }

  public readonly rastrearColuna = (indice: number, coluna: ColunaGridDados): string => {
    void indice;
    return coluna.campo;
  };

  public readonly rastrearLinha = (indice: number, linha: unknown): number | string => {
    if (linha && typeof linha === 'object' && 'id' in (linha as Record<string, unknown>)) {
      const valor = (linha as Record<string, unknown>)['id'];
      if (typeof valor === 'number' || typeof valor === 'string') {
        return valor;
      }
    }

    return indice;
  };
}
