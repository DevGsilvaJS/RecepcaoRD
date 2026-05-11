/**
 * Espelha o envelope ApiResponse da API (serialização camelCase).
 */
export interface RespostaApi<T> {
  sucesso: boolean;
  mensagem?: string | null;
  dados?: T | null;
  erros?: readonly string[] | null;
}
