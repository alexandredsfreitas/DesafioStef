namespace Questao5.Domain.Language;

public static class ErrorMessages
{
    public const string ContaNaoEncontrada = "Conta corrente não encontrada.";
    public const string ContaInativa = "Conta corrente não está ativa.";
    public const string ValorInvalido = "Valor da movimentação deve ser maior que zero.";
    public const string TipoMovimentoInvalido = "Tipo de movimento inválido. Use 'C' para crédito ou 'D' para débito.";
}