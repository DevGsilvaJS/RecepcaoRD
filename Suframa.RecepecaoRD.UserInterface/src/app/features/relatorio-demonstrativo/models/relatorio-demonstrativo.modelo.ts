export interface FiltrosRelatorioDemonstrativo {
  anoBase?: number;
  situacaoAtual?: string;
}

export interface LinhaRelatorioDemonstrativoDto {
  id: number;
  codigoRd: string;
  tipoRd: string;
  tipoRdNumero: number;
  cnpj: string;
  razaoSocial: string;
  anoBase: number;
  planoId: number;
  numeroPlano: string;
  representanteLegal: string;
  telefone: string;
  email: string;
  dataCriacaoUtc: string;
}
