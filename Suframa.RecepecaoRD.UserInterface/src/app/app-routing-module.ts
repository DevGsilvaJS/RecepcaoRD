import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ROTA_AREA_AUTENTICADA, ROTA_LOGIN } from './core/constants/constantes-app.constante';
import { AutenticacaoGuard } from './core/guards/autenticacao.guard';
import { LayoutComponent } from './layout/main-layout/layout.component';
import { LoginPage } from './features/autenticacao/pages/login.page';
import { ConveniosPage } from './features/convenios/pages/convenios.page';
import { PagamentoSaldoResidualPage } from './features/pagamento-saldo-residual/pages/pagamento-saldo-residual.page';
import { PlanoDeTrabalhoPage } from './features/plano-de-trabalho/pages/plano-de-trabalho.page';
import { QuitacaoSaldoDevedorPage } from './features/quitacao-saldo-devedor/pages/quitacao-saldo-devedor.page';
import { ListagemRelatorioDemonstrativoPage } from './features/relatorio-demonstrativo/pages/listagem-relatorio-demonstrativo/listagem-relatorio-demonstrativo.page';
import { CadastroRelatorioDemonstrativoPage } from './features/relatorio-demonstrativo/pages/cadastro-relatorio-demonstrativo/cadastro-relatorio-demonstrativo.page';
import { EdicaoRelatorioDemonstrativoPage } from './features/relatorio-demonstrativo/pages/edicao-relatorio-demonstrativo/edicao-relatorio-demonstrativo.page';
import { RelatorioDemonstrativoRotaPage } from './features/relatorio-demonstrativo/pages/relatorio-demonstrativo-rota.page';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: ROTA_LOGIN },
  { path: ROTA_LOGIN, component: LoginPage },
  {
    path: ROTA_AREA_AUTENTICADA,
    component: LayoutComponent,
    canActivateChild: [AutenticacaoGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'relatorio-demonstrativo' },
      { path: 'plano-de-trabalho', component: PlanoDeTrabalhoPage },
      { path: 'convenios', component: ConveniosPage },
      {
        path: 'relatorio-demonstrativo',
        component: RelatorioDemonstrativoRotaPage,
        children: [
          { path: '', component: ListagemRelatorioDemonstrativoPage },
          { path: 'cadastro-relatorio-demonstrativo', component: CadastroRelatorioDemonstrativoPage },
          { path: 'edicao-relatorio-demonstrativo', component: EdicaoRelatorioDemonstrativoPage },
        ],
      },
      { path: 'pagamento-saldo-residual', component: PagamentoSaldoResidualPage },
      { path: 'quitacao-saldo-devedor', component: QuitacaoSaldoDevedorPage },
    ],
  },
  { path: '**', redirectTo: ROTA_LOGIN },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
