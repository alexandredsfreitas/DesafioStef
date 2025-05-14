using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.CommandStore.Requests;

public interface IMovimentoCommandStore
{
    Task<ContaCorrente> ObterContaCorrentePorId(string idContaCorrente);
    Task<string> InserirMovimento(Movimento movimento);
    Task<Idempotencia> ObterIdempotenciaPorChave(string chaveIdempotencia);
    Task InserirIdempotencia(Idempotencia idempotencia);
}