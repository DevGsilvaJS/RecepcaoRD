import { Directive, Input, TemplateRef } from '@angular/core';

export type NomeTemplateGrid = 'botoes' | 'filtros' | 'acoesLinha' | 'celula';

@Directive({
  selector: 'ng-template[appTemplateGrid]',
  standalone: false,
})
export class TemplateGridDirective {
  @Input('appTemplateGrid')
  public nomeTemplate!: NomeTemplateGrid;

  public constructor(public readonly templateRef: TemplateRef<unknown>) { }
}
