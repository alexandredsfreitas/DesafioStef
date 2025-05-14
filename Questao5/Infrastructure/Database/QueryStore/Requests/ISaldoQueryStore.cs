using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.QueryStore.Requests;

public interface ISaldoQueryStore
{
    Task<ContaCorrente> ObterContaCorrentePorId(string idContaCorrente);
    Task<IEnumerable<Movimento>> ObterMovimentosPorContaCorrente(string idContaCorrente);
}