using MediatR;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Language;
using Questao5.Infrastructure.Database.QueryStore.Requests;

namespace Questao5.Application.Handlers;

public class SaldoHandler : IRequestHandler<SaldoRequest, object>
{
    private readonly ISaldoQueryStore _saldoQueryStore;

    public SaldoHandler(ISaldoQueryStore saldoQueryStore)
    {
        _saldoQueryStore = saldoQueryStore;
    }

    public async Task<object> Handle(SaldoRequest request, CancellationToken cancellationToken)
    {
        // Valida a conta corrente
        var contaCorrente = await _saldoQueryStore.ObterContaCorrentePorId(request.IdContaCorrente);
        if (contaCorrente == null)
        {
            return new ErrorResponse
            {
                Tipo = ErrorTypes.InvalidAccount,
                Mensagem = ErrorMessages.ContaNaoEncontrada
            };
        }

        // Valida se a conta está ativa
        if (!contaCorrente.Ativo)
        {
            return new ErrorResponse
            {
                Tipo = ErrorTypes.InactiveAccount,
                Mensagem = ErrorMessages.ContaInativa
            };
        }

        // Obte os movimentos da conta
        var movimentos = await _saldoQueryStore.ObterMovimentosPorContaCorrente(request.IdContaCorrente);

        // Calcula o saldo
        decimal saldo = 0;
        foreach (var movimento in movimentos)
        {
            if (movimento.TipoMovimento == TipoMovimento.Credito)
            {
                saldo += movimento.Valor;
            }
            else if (movimento.TipoMovimento == TipoMovimento.Debito)
            {
                saldo -= movimento.Valor;
            }
        }

        var response = new SaldoResponse
        {
            NumeroContaCorrente = contaCorrente.Numero,
            NomeTitular = contaCorrente.Nome,
            DataHoraConsulta = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Saldo = saldo
        };

        return response;
    }
}