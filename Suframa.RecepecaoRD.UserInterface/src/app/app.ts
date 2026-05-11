import { Component, signal } from '@angular/core';

import { NOME_APP } from './core/constants/constantes-app.constante';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
})
export class App {
  protected readonly title = signal(NOME_APP);
}
