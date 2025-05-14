using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.QueryStore.Requests;

public class SaldoQueryStore : ISaldoQueryStore
{
    private readonly DatabaseConfig _databaseConfig;

    public SaldoQueryStore(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    public async Task<ContaCorrente> ObterContaCorrentePorId(string idContaCorrente)
    {
        using var connection = new SqliteConnection(_databaseConfig.Name);

        var query = @"SELECT idcontacorrente as IdContaCorrente, 
                                 numero as Numero, 
                                 nome as Nome, 
                                 ativo as Ativo 
                          FROM contacorrente 
                          WHERE idcontacorrente = @IdContaCorrente";

        var contaCorrente = await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
            query,
            new { IdContaCorrente = idContaCorrente }
        );

        return contaCorrente;
    }

    public async Task<IEnumerable<Movimento>> ObterMovimentosPorContaCorrente(string idContaCorrente)
    {
        using var connection = new SqliteConnection(_databaseConfig.Name);

        var query = @"SELECT idmovimento as IdMovimento, 
                                 idcontacorrente as IdContaCorrente, 
                                 datamovimento as DataMovimento, 
                                 tipomovimento as TipoMovimento, 
                                 valor as Valor 
                          FROM movimento 
                          WHERE idcontacorrente = @IdContaCorrente";

        var movimentos = await connection.QueryAsync<Movimento>(
            query,
            new { IdContaCorrente = idContaCorrente }
        );

        return movimentos;
    }
}