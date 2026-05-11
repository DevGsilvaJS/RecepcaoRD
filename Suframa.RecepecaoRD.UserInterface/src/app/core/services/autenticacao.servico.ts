import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, map, of, throwError } from 'rxjs';
import { EmpresaSessao } from '../models/empresa-sessao.modelo';
import { ServicoApi } from './servico-api.servico';

type EmpresaApi = Readonly<{
  cnpj?: string;
  Cnpj?: string;
  inscricaoSuframa?: string;
  InscricaoSuframa?: string;
  razaoSocial?: string;
  RazaoSocial?: string;
  endereco?: string;
  Endereco?: string;
}>;

type RespostaLoginCookieApi = Readonly<{
  mensagem?: string;
  Mensagem?: string;
  expiraEmJwtUtc?: string | null;
  ExpiraEmJwtUtc?: string | null;
  empresa?: EmpresaApi | null;
  Empresa?: EmpresaApi | null;
}>;

type SessaoAutenticacaoApi = Readonly<{
  autenticado?: boolean;
  Autenticado?: boolean;
  expiraEmJwtUtc?: string | null;
  ExpiraEmJwtUtc?: string | null;
}>;

export type RespostaMensagemApi = Readonly<{
  mensagem: string;
  expiraEmJwtUtc?: string | null;
}>;

export type RespostaLoginCompletaApi = Readonly<{
  mensagem: string;
  expiraEmJwtUtc: string | null;
  empresa: EmpresaSessao | null;
}>;

export type SessaoApi = Readonly<{
  autenticado: boolean;
  expiraEmJwtUtc?: string | null;
}>;

@Injectable({ providedIn: 'root' })
export class AutenticacaoServico {
  constructor(private readonly servicoApi: ServicoApi) {}

  autenticar(documento: string): Observable<RespostaLoginCompletaApi> {
    const documentoNormalizado = (documento ?? '').replace(/\D/g, '').trim();
    if (!documentoNormalizado) {
      return throwError(
        () => new Error('Informe o CPF (11 dígitos) ou o CNPJ (14 dígitos) para autenticar.'),
      );
    }

    return this.servicoApi
      .get<RespostaLoginCookieApi>('/api/autenticacao', {
        parametros: { document: documentoNormalizado },
      })
      .pipe(
      map((corpo) => {
        const mensagem = (corpo?.mensagem ?? corpo?.Mensagem ?? 'Sessão iniciada.').trim();
        const expiraEmJwtUtc =
          (corpo?.expiraEmJwtUtc ?? corpo?.ExpiraEmJwtUtc ?? null) as string | null;
        const empresa = this.normalizarEmpresa({
          empresa: corpo?.empresa ?? corpo?.Empresa ?? null,
        });
        return {
          mensagem,
          expiraEmJwtUtc: expiraEmJwtUtc ?? null,
          empresa,
        } satisfies RespostaLoginCompletaApi;
      }),
      catchError((erro: unknown) => {
        const mensagem = this.extrairMensagemErroHttp(erro);
        return throwError(() => new Error(mensagem));
      }),
    );
  }

  normalizarEmpresa(resposta?: Readonly<{ empresa?: EmpresaApi | null; Empresa?: EmpresaApi | null }> | null): EmpresaSessao | null {
    const empresa = resposta?.empresa ?? resposta?.Empresa ?? null;
    if (!empresa) {
      return null;
    }

    const cnpj = (empresa.cnpj ?? empresa.Cnpj ?? '').trim();
    const inscricaoSuframa = (empresa.inscricaoSuframa ?? empresa.InscricaoSuframa ?? '').trim();
    const razaoSocial = (empresa.razaoSocial ?? empresa.RazaoSocial ?? '').trim();
    const endereco = (empresa.endereco ?? empresa.Endereco ?? '').trim();

    if (!cnpj) {
      return null;
    }

    return {
      cnpj,
      inscricaoSuframa,
      razaoSocial,
      endereco,
    };
  }

  renovarSessao(): Observable<RespostaMensagemApi> {
    return this.servicoApi
      .post<RespostaMensagemApi, Readonly<Record<string, never>>>('/api/autenticacao/refresh', {})
      .pipe(
        map((corpo) => ({
          mensagem: (corpo?.mensagem ?? 'Sessão renovada.').trim(),
          expiraEmJwtUtc: corpo?.expiraEmJwtUtc ?? null,
        })),
        catchError((erro: unknown) => {
          if (erro instanceof HttpErrorResponse && erro.status === 401) {
            return throwError(() => erro);
          }
          const mensagem = this.extrairMensagemErroHttp(erro);
          return throwError(() => new Error(mensagem));
        }),
      );
  }

  sair(): Observable<RespostaMensagemApi> {
    return this.servicoApi
      .post<RespostaMensagemApi, Readonly<Record<string, never>>>('/api/autenticacao/logout', {})
      .pipe(
        map((corpo) => ({
          mensagem: (corpo?.mensagem ?? 'Sessão encerrada.').trim(),
        })),
        catchError(() => of({ mensagem: 'Sessão encerrada (offline ou indisponível).' })),
      );
  }

  obterSessao(): Observable<SessaoApi> {
    return this.servicoApi.get<SessaoAutenticacaoApi>('/api/autenticacao/sessao').pipe(
      map((corpo) => {
        const autenticado = corpo?.autenticado === true || corpo?.Autenticado === true;
        const expiraEmJwtUtc = (corpo?.expiraEmJwtUtc ?? corpo?.ExpiraEmJwtUtc ?? null) as string | null;
        return {
          autenticado,
          expiraEmJwtUtc: expiraEmJwtUtc ?? undefined,
        };
      }),
      catchError((erro: unknown) => {
        if (erro instanceof HttpErrorResponse && erro.status === 401) {
          return of({ autenticado: false });
        }
        return throwError(() => erro);
      }),
    );
  }

  estaAutenticado(): Observable<boolean> {
    return this.obterSessao().pipe(
      map((sessao) => sessao?.autenticado === true),
      catchError(() => of(false)),
    );
  }

  private extrairMensagemErroHttp(erro: unknown): string {
    if (erro instanceof HttpErrorResponse) {
      const corpo = erro.error;
      if (typeof corpo === 'string' && corpo.trim()) {
        return corpo.trim();
      }
      if (corpo && typeof corpo === 'object') {
        const o = corpo as Record<string, unknown>;
        const detail = o['detail'];
        if (typeof detail === 'string' && detail.trim()) {
          return detail.trim();
        }
        const title = o['title'];
        if (typeof title === 'string' && title.trim()) {
          return title.trim();
        }
        const message = o['message'];
        if (typeof message === 'string' && message.trim()) {
          return message.trim();
        }
      }
      if (erro.status === 404) {
        return 'Endpoint de autenticação não encontrado.';
      }
      if (erro.status === 400) {
        return 'Dados de autenticação inválidos.';
      }
    }
    if (erro instanceof Error && erro.message) {
      return erro.message;
    }
    return 'Falha ao autenticar.';
  }
}
