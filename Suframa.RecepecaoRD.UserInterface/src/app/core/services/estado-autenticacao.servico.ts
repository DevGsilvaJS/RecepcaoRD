import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, map, of, take } from 'rxjs';
import { EmpresaSessao } from '../models/empresa-sessao.modelo';
import { AutenticacaoServico } from './autenticacao.servico';

@Injectable({ providedIn: 'root' })
export class EstadoAutenticacaoServico {
  private static readonly chaveEmpresaSessao: string = 'sagat_empresa_sessao';

  private readonly autenticadoSubject = new BehaviorSubject<boolean>(false);
  private readonly expiraEmJwtUtcSubject = new BehaviorSubject<Date | null>(null);
  private readonly empresaSubject = new BehaviorSubject<EmpresaSessao | null>(this.obterEmpresaArmazenada());
  private inicializado: boolean = false;

  public readonly autenticado$: Observable<boolean> = this.autenticadoSubject.asObservable();
  public readonly expiraEmJwtUtc$: Observable<Date | null> = this.expiraEmJwtUtcSubject.asObservable();
  public readonly empresa$: Observable<EmpresaSessao | null> = this.empresaSubject.asObservable();

  constructor(private readonly autenticacaoServico: AutenticacaoServico) {}

  obterAutenticadoSnapshot(): boolean {
    return this.autenticadoSubject.value;
  }

  obterEmpresaSnapshot(): EmpresaSessao | null {
    return this.empresaSubject.value;
  }

  inicializar(): Observable<boolean> {
    if (this.inicializado) {
      return of(this.autenticadoSubject.value);
    }

    this.inicializado = true;
    return this.autenticacaoServico.obterSessao().pipe(
      map((sessao) => {
        const autenticado = sessao?.autenticado === true;
        this.expiraEmJwtUtcSubject.next(this.converterDataUtc(sessao?.expiraEmJwtUtc));
        return autenticado;
      }),
      catchError(() => of(false)),
      map((autenticado) => {
        this.autenticadoSubject.next(autenticado);
        return autenticado;
      }),
      take(1),
    );
  }

  marcarComoAutenticado(expiraEmJwtUtc?: string | null, empresa?: EmpresaSessao | null): void {
    this.autenticadoSubject.next(true);
    if (expiraEmJwtUtc !== undefined) {
      this.expiraEmJwtUtcSubject.next(this.converterDataUtc(expiraEmJwtUtc));
    }

    if (empresa !== undefined) {
      this.definirEmpresa(empresa);
    }
  }

  marcarComoDesautenticado(): void {
    this.autenticadoSubject.next(false);
    this.expiraEmJwtUtcSubject.next(null);
    this.definirEmpresa(null);
  }

  obterAtrasoRenovacaoMs(margemMs: number = 2 * 60 * 1000): number | null {
    const expiraEm = this.expiraEmJwtUtcSubject.value;
    if (!expiraEm) {
      return null;
    }

    const ms = expiraEm.getTime() - Date.now() - margemMs;
    return Math.max(0, ms);
  }

  private converterDataUtc(valor?: string | null): Date | null {
    if (!valor) {
      return null;
    }

    const data = new Date(valor);
    return Number.isNaN(data.getTime()) ? null : data;
  }

  private definirEmpresa(empresa: EmpresaSessao | null): void {
    this.empresaSubject.next(empresa);
    this.salvarEmpresa(empresa);
  }

  private obterEmpresaArmazenada(): EmpresaSessao | null {
    try {
      const bruto = sessionStorage.getItem(EstadoAutenticacaoServico.chaveEmpresaSessao);
      if (!bruto) {
        return null;
      }

      const json = JSON.parse(bruto) as Partial<EmpresaSessao>;
      const cnpj = (json.cnpj ?? '').trim();
      if (!cnpj) {
        return null;
      }

      return {
        cnpj,
        inscricaoSuframa: (json.inscricaoSuframa ?? '').trim(),
        razaoSocial: (json.razaoSocial ?? '').trim(),
        endereco: (json.endereco ?? '').trim(),
      };
    } catch {
      return null;
    }
  }

  private salvarEmpresa(empresa: EmpresaSessao | null): void {
    try {
      if (!empresa) {
        sessionStorage.removeItem(EstadoAutenticacaoServico.chaveEmpresaSessao);
        return;
      }

      sessionStorage.setItem(EstadoAutenticacaoServico.chaveEmpresaSessao, JSON.stringify(empresa));
    } catch {
      // sem ação: storage pode estar indisponível
    }
  }
}
