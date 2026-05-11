import { APP_INITIALIZER, NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { firstValueFrom } from 'rxjs';
import { provideHotToastConfig } from '@ngxpert/hot-toast';

import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { CnpjConsultaInterceptor } from './core/interceptors/cnpj-consulta.interceptor';
import { CredenciaisCookieInterceptor } from './core/interceptors/credenciais-cookie.interceptor';
import { RenovacaoSessaoInterceptor } from './core/interceptors/renovacao-sessao.interceptor';
import { EstadoAutenticacaoServico } from './core/services/estado-autenticacao.servico';
import { RenovacaoSilenciosaServico } from './core/services/renovacao-silenciosa.servico';
import { LayoutModule } from './layout/layout.module';
import { SharedModule } from './shared/shared.module';
import { LoginPage } from './features/autenticacao/pages/login.page';
import { ConveniosPage } from './features/convenios/pages/convenios.page';
import { PagamentoSaldoResidualPage } from './features/pagamento-saldo-residual/pages/pagamento-saldo-residual.page';
import { PlanoDeTrabalhoPage } from './features/plano-de-trabalho/pages/plano-de-trabalho.page';
import { QuitacaoSaldoDevedorPage } from './features/quitacao-saldo-devedor/pages/quitacao-saldo-devedor.page';
import { ListagemRelatorioDemonstrativoPage } from './features/relatorio-demonstrativo/pages/listagem-relatorio-demonstrativo/listagem-relatorio-demonstrativo.page';
import { GridRelatorioDemonstrativoComponent } from './features/relatorio-demonstrativo/components/grid-relatorio-demonstrativo/grid-relatorio-demonstrativo.component';
import { CadastroRelatorioDemonstrativoPage } from './features/relatorio-demonstrativo/pages/cadastro-relatorio-demonstrativo/cadastro-relatorio-demonstrativo.page';
import { EdicaoRelatorioDemonstrativoPage } from './features/relatorio-demonstrativo/pages/edicao-relatorio-demonstrativo/edicao-relatorio-demonstrativo.page';
import { RelatorioDemonstrativoRotaPage } from './features/relatorio-demonstrativo/pages/relatorio-demonstrativo-rota.page';

export function inicializarEstadoAutenticacao(estado: EstadoAutenticacaoServico): () => Promise<boolean> {
  return () => firstValueFrom(estado.inicializar());
}

export function iniciarRenovacaoSilenciosa(renovacaoSilenciosa: RenovacaoSilenciosaServico): () => void {
  return () => renovacaoSilenciosa.iniciar();
}

@NgModule({
  declarations: [
    App,
    LoginPage,
    PlanoDeTrabalhoPage,
    ConveniosPage,
    RelatorioDemonstrativoRotaPage,
    ListagemRelatorioDemonstrativoPage,
    GridRelatorioDemonstrativoComponent,
    CadastroRelatorioDemonstrativoPage,
    EdicaoRelatorioDemonstrativoPage,
    PagamentoSaldoResidualPage,
    QuitacaoSaldoDevedorPage,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    LayoutModule,
    SharedModule,
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
    provideHotToastConfig(),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: '.modo-escuro',
        },
      },
    }),
    { provide: APP_INITIALIZER, useFactory: inicializarEstadoAutenticacao, deps: [EstadoAutenticacaoServico], multi: true },
    { provide: APP_INITIALIZER, useFactory: iniciarRenovacaoSilenciosa, deps: [RenovacaoSilenciosaServico], multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: CredenciaisCookieInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: RenovacaoSessaoInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: CnpjConsultaInterceptor, multi: true },
  ],
  bootstrap: [App]
})
export class AppModule { }
