import { Component, Input, OnInit } from '@angular/core';
import { AplicacaoServico } from '../../../shared/services/aplicacao.servico';
import { environment } from '../../../../environments/environment';

type MenuFuncao = {
  nome: string;
  url: string;
  descricao: string;
  icon?: 'plano' | 'convenios' | 'relatorio' | 'pagamento' | 'quitacao';
};

type MenuGrupo = {
  nome: string;
  id: number;
  funcoesSistema: MenuFuncao[];
};

interface UsuarioLogadoResposta {
  usuNomeRepresentanteLogado?: string;
  usuNomeUsuario?: string;
  usuCpfRepresentanteLogado?: string;
  usuCpfCnpj?: string;
}

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  standalone: false,
})
export class MenuComponent implements OnInit {
  @Input() showBrand = true;

  public Pss: MenuGrupo[] = [];

  usuario = 'Usuário';
  cpfcnpj = 'Empresa ABC';

  get initials(): string {
    const parts = (this.usuario ?? '')
      .trim()
      .split(/\s+/g)
      .filter(Boolean);

    const first = parts[0]?.[0] ?? 'U';
    const last = parts.length > 1 ? parts[parts.length - 1]?.[0] : parts[0]?.[1];
    return `${first}${last ?? ''}`.toUpperCase();
  }

  constructor(private readonly aplicacaoServico: AplicacaoServico) {}

  ngOnInit(): void {
    this.aplicacaoServico.get<UsuarioLogadoResposta>('UsuarioLogado').subscribe((result) => {
      if (!result) return;

      this.usuario = result.usuNomeUsuario ?? result.usuNomeRepresentanteLogado ?? this.usuario;
      this.cpfcnpj = result.usuCpfCnpj ?? result.usuCpfRepresentanteLogado ?? this.cpfcnpj;

      if (environment.developmentMode) {
        this.montarMenuMock();
      } else {
        this.buscaMenuUsuarioLogado();
      }
    });
  }

  buscaMenuUsuarioLogado(): void {
    this.aplicacaoServico.get<MenuGrupo[]>('menu').subscribe((result) => {
      if (Array.isArray(result)) this.Pss = result;
    });
  }

  montarMenuMock(): void {
    this.Pss = [
      {
        nome: 'Navegação',
        id: 1,
        funcoesSistema: [
          {
            nome: 'Planos de PD&I',
            url: '/app/plano-de-trabalho',
            descricao: 'Planos de PD&I',
            icon: 'plano',
          },
          {
            nome: 'Convênios',
            url: '/app/convenios',
            descricao: 'Convênios',
            icon: 'convenios',
          },
          {
            nome: 'Relatório Demonstrativo',
            url: '/app/relatorio-demonstrativo',
            descricao: 'Relatório Demonstrativo',
            icon: 'relatorio',
          },
          {
            nome: 'Pagamento de Saldo Residual',
            url: '/app/pagamento-saldo-residual',
            descricao: 'Pagamento de Saldo Residual',
            icon: 'pagamento',
          },
          {
            nome: 'Quitação de Saldo Devedor',
            url: '/app/quitacao-saldo-devedor',
            descricao: 'Quitação de Saldo Devedor',
            icon: 'quitacao',
          },
        ],
      },
    ];
  }
}
