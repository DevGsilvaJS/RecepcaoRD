import { DOCUMENT } from '@angular/common';
import { AfterViewInit, Component, Inject, Input, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-cabecalho-governo-federal',
  templateUrl: './cabecalho-governo-federal.component.html',
  standalone: false,
})
export class CabecalhoGovernoFederalComponent implements AfterViewInit {
  @Input() enabled = false;

  constructor(
    private renderer: Renderer2,
    @Inject(DOCUMENT) private document: Document,
  ) {}

  ngAfterViewInit(): void {
    if (typeof window === 'undefined') return;
    if (!this.enabled) return;

    const maybeLoadBarra = () => {
      this.ensureScript({
        id: 'barra-brasil-js',
        src: '//barra.brasil.gov.br/barra.js',
        defer: true,
      });
    };

    const hasJquery = (window as Window & { jQuery?: unknown; $?: unknown }).jQuery
      ?? (window as Window & { jQuery?: unknown; $?: unknown }).$;
    if (hasJquery) {
      maybeLoadBarra();
      return;
    }

    this.ensureScript({
      id: 'jquery-js',
      src: 'https://code.jquery.com/jquery-3.7.1.min.js',
      defer: true,
      onload: maybeLoadBarra,
      onerror: maybeLoadBarra,
    });
  }

  private ensureScript(args: {
    id: string;
    src: string;
    defer?: boolean;
    onload?: () => void;
    onerror?: () => void;
  }): void {
    if (this.document.getElementById(args.id)) return;

    const script = this.renderer.createElement('script') as HTMLScriptElement;
    script.id = args.id;
    script.type = 'text/javascript';
    script.src = args.src;
    if (args.defer) script.defer = true;
    if (args.onload) script.onload = args.onload;
    if (args.onerror) script.onerror = args.onerror;

    this.renderer.appendChild(this.document.body, script);
  }
}
