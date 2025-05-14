using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.CommandStore.Requests;

public class MovimentoCommandStore : IMovimentoCommandStore
{
    private readonly DatabaseConfig _databaseConfig;

    public MovimentoCommandStore(DatabaseConfig databaseConfig)
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

    public async Task<string> InserirMovimento(Movimento movimento)
    {
        using var connection = new SqliteConnection(_databaseConfig.Name);

        var query = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                     VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

        await connection.ExecuteAsync(
            query,
            new
            {
                movimento.IdMovimento,
                movimento.IdContaCorrente,
                movimento.DataMovimento,
                movimento.TipoMovimento,
                movimento.Valor
            }
        );

        return movimento.IdMovimento;
    }

    public async Task<Idempotencia> ObterIdempotenciaPorChave(string chaveIdempotencia)
    {
        using var connection = new SqliteConnection(_databaseConfig.Name);

        var query = @"SELECT chave_idempotencia as ChaveIdempotencia, 
                             requisicao as Requisicao, 
                             resultado as Resultado 
                      FROM idempotencia 
                      WHERE chave_idempotencia = @ChaveIdempotencia";

        var idempotencia = await connection.QueryFirstOrDefaultAsync<Idempotencia>(
            query,
            new { ChaveIdempotencia = chaveIdempotencia }
        );

        return idempotencia;
    }

    public async Task InserirIdempotencia(Idempotencia idempotencia)
    {
        using var connection = new SqliteConnection(_databaseConfig.Name);

        var query = @"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                     VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";

        await connection.ExecuteAsync(
            query,
            new
            {
                idempotencia.ChaveIdempotencia,
                idempotencia.Requisicao,
                idempotencia.Resultado
            }
        );
    }
}