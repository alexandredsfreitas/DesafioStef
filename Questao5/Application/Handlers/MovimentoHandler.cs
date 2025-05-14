using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Language;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using System.Text.Json;

namespace Questao5.Application.Handlers;

public class MovimentoHandler : IRequestHandler<MovimentoRequest, object>
{
    private readonly IMovimentoCommandStore _movimentoCommandStore;

    public MovimentoHandler(IMovimentoCommandStore movimentoCommandStore)
    {
        _movimentoCommandStore = movimentoCommandStore;
    }

    public async Task<object> Handle(MovimentoRequest request, CancellationToken cancellationToken)
    {
        // Verifica idempotência
        var idempotencia = await _movimentoCommandStore.ObterIdempotenciaPorChave(request.IdRequisicao);
        if (idempotencia != null)
        {
            // Se a requisição já foi processada, retorna o resultado anterior
            return JsonSerializer.Deserialize<object>(idempotencia.Resultado);
        }

        // Valida a conta corrente
        var contaCorrente = await _movimentoCommandStore.ObterContaCorrentePorId(request.IdContaCorrente);
        if (contaCorrente == null)
        {
            var errorResponse = new ErrorResponse
            {
                Tipo = ErrorTypes.InvalidAccount,
                Mensagem = ErrorMessages.ContaNaoEncontrada
            };

            await SalvarIdempotencia(request, errorResponse);
            return errorResponse;
        }

        // Valida se a conta está ativa
        if (!contaCorrente.Ativo)
        {
            var errorResponse = new ErrorResponse
            {
                Tipo = ErrorTypes.InactiveAccount,
                Mensagem = ErrorMessages.ContaInativa
            };

            await SalvarIdempotencia(request, errorResponse);
            return errorResponse;
        }

        // Valida o valor
        if (request.Valor <= 0)
        {
            var errorResponse = new ErrorResponse
            {
                Tipo = ErrorTypes.InvalidValue,
                Mensagem = ErrorMessages.ValorInvalido
            };

            await SalvarIdempotencia(request, errorResponse);
            return errorResponse;
        }

        // Valida o tipo de movimento
        if (request.TipoMovimento != TipoMovimento.Credito && request.TipoMovimento != TipoMovimento.Debito)
        {
            var errorResponse = new ErrorResponse
            {
                Tipo = ErrorTypes.InvalidType,
                Mensagem = ErrorMessages.TipoMovimentoInvalido
            };

            await SalvarIdempotencia(request, errorResponse);
            return errorResponse;
        }

        // Cria o movimento
        var movimento = new Movimento
        {
            IdMovimento = Guid.NewGuid().ToString(),
            IdContaCorrente = request.IdContaCorrente,
            DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
            TipoMovimento = request.TipoMovimento,
            Valor = request.Valor
        };

        // Persiste o movimento
        await _movimentoCommandStore.InserirMovimento(movimento);

        var response = new MovimentoResponse
        {
            IdMovimento = movimento.IdMovimento
        };

        // Idempotência final
        await SalvarIdempotencia(request, response);

        return response;
    }

    private async Task SalvarIdempotencia(MovimentoRequest request, object response)
    {
        var idempotencia = new Idempotencia
        {
            ChaveIdempotencia = request.IdRequisicao,
            Requisicao = JsonSerializer.Serialize(request),
            Resultado = JsonSerializer.Serialize(response)
        };

        await _movimentoCommandStore.InserirIdempotencia(idempotencia);
    }
}