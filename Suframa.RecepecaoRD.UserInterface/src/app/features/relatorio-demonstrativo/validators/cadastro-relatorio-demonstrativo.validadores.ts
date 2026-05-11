/** Validações específicas do cadastro de RD (campos de contato). */

export function validarEmailCadastroRd(email: string): boolean {
  const texto = (email ?? '').trim();
  if (!texto) return false;
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(texto);
}

export function validarTelefoneCadastroRd(telefone: string): boolean {
  const texto = (telefone ?? '').trim();
  if (!texto) return false;
  return /^\(\d{2}\)\s\d{5}-\d{4}$/.test(texto);
}
