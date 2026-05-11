import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { InputMaskModule } from 'primeng/inputmask';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { GridComponent } from './components/grade-dados/grade-dados.component';
import { TemplateGridDirective } from './directives/template-grade-dados.directive';

@NgModule({
  declarations: [GridComponent, TemplateGridDirective],
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputMaskModule,
    InputTextModule,
    PaginatorModule,
    SelectModule,
    TableModule,
    TagModule,
  ],
  exports: [
    GridComponent,
    TemplateGridDirective,
    FormsModule,
    ButtonModule,
    InputMaskModule,
    InputTextModule,
    PaginatorModule,
    SelectModule,
    TableModule,
    TagModule,
  ],
})
export class SharedModule {}
