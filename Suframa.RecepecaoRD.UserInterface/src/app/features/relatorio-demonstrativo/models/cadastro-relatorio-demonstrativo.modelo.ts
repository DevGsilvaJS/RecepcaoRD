export type PlanoOpcaoDto = Readonly<{
  id: number;
  numeroPlano: string;
}>;

export interface CriarRdRecepcaoDto {
  cnpj: string;
  razaoSocial: string;
  endereco: string;
  inscricaoSuframa: string;
  anoBase: number;
  tipoRd: number;
  planoId: number;
  representanteLegal: string;
  telefone: string;
  email: string;
  idOrigem?: number | null;
}

export type AtualizarRdRecepcaoDto = Readonly<{
  anoBase: number;
  planoId: number;
  representanteLegal: string;
  telefone: string;
  email: string;
}>;
