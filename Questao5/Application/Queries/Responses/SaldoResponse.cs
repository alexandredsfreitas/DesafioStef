namespace Questao5.Application.Queries.Responses;

public class SaldoResponse
{
    public int NumeroContaCorrente { get; set; }
    public string NomeTitular { get; set; }
    public string DataHoraConsulta { get; set; }
    public decimal Saldo { get; set; }
}