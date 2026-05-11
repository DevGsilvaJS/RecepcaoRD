import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { CabecalhoGovernoFederalComponent } from './header/cabecalho-governo-federal/cabecalho-governo-federal.component';
import { CabecalhoSuframaComponent } from './header/cabecalho-suframa/cabecalho-suframa.component';
import { LayoutComponent } from './main-layout/layout.component';
import { MenuComponent } from './sidebar/menu/menu.component';
import { RodapeGovernoFederalComponent } from './footer/rodape-governo-federal/rodape-governo-federal.component';
import { RodapeMenuComponent } from './footer/rodape-menu/rodape-menu.component';

@NgModule({
  declarations: [
    LayoutComponent,
    MenuComponent,
    CabecalhoGovernoFederalComponent,
    CabecalhoSuframaComponent,
    RodapeGovernoFederalComponent,
    RodapeMenuComponent,
  ],
  imports: [CommonModule, RouterModule],
  exports: [LayoutComponent],
})
export class LayoutModule {}
