import { RespostaApi } from '../models/resposta-api.modelo';

export function obterMensagemErroRespostaApi(resposta: RespostaApi<unknown>): string {
  const deLista = resposta.erros?.map((e) => e.trim()).filter(Boolean).join(' ');
  return (resposta.mensagem ?? '').trim() || (deLista ?? '').trim() || 'Operação não concluída.';
}
