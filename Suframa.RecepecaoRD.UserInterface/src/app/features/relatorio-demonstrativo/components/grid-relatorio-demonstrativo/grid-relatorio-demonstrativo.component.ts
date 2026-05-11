import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';

import { ColunaGridDados } from '../../../../shared/interfaces/coluna-grid-dados.interface';
import { LinhaRelatorioDemonstrativoDto } from '../../models/relatorio-demonstrativo.modelo';

@Component({
  selector: 'app-grid-relatorio-demonstrativo',
  templateUrl: './grid-relatorio-demonstrativo.component.html',
  standalone: false,
})
export class GridRelatorioDemonstrativoComponent {
  @Input({ required: true })
  public titulo!: string;

  @Input()
  public subtitulo?: string;

  @Input({ required: true })
  public colunas!: ColunaGridDados[];

  @Input({ required: true })
  public dados!: LinhaRelatorioDemonstrativoDto[];

  @Input()
  public total: number = 0;

  @Input()
  public page: number = 1;

  @Input()
  public size: number = 10;

  @Input()
  public arrayPageSizeOptions: number[] = [10, 20, 50];

  @Input()
  public templateFiltros?: TemplateRef<unknown>;

  @Output()
  public readonly onChangePage: EventEmitter<number> = new EventEmitter<number>();

  @Output()
  public readonly onChangeSize: EventEmitter<number> = new EventEmitter<number>();

  @Output()
  public readonly onNovo: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public readonly onExportar: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public readonly onEditar: EventEmitter<LinhaRelatorioDemonstrativoDto> = new EventEmitter<LinhaRelatorioDemonstrativoDto>();

  @Output()
  public readonly onExcluir: EventEmitter<LinhaRelatorioDemonstrativoDto> = new EventEmitter<LinhaRelatorioDemonstrativoDto>();

  @Output()
  public readonly onCopiar: EventEmitter<LinhaRelatorioDemonstrativoDto> = new EventEmitter<LinhaRelatorioDemonstrativoDto>();

  @Output()
  public readonly onAbrirDetalhes: EventEmitter<LinhaRelatorioDemonstrativoDto> = new EventEmitter<LinhaRelatorioDemonstrativoDto>();
}
