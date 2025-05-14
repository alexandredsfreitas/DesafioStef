using NSubstitute;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Database.QueryStore.Requests;

namespace Questao5.Tests;

public class SaldoHandlerTests
{
    private readonly ISaldoQueryStore _saldoQueryStore;
    private readonly SaldoHandler _handler;

    public SaldoHandlerTests()
    {
        _saldoQueryStore = Substitute.For<ISaldoQueryStore>();
        _handler = new SaldoHandler(_saldoQueryStore);
    }

    [Fact]
    public async Task Handle_ContaInexistente_DeveRetornarInvalidAccount()
    {
        // Arrange
        var request = new SaldoRequest
        {
            IdContaCorrente = "ContaInexistente"
        };

        _saldoQueryStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns((ContaCorrente)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var errorResponse = result as ErrorResponse;
        Assert.NotNull(errorResponse);
        Assert.Equal(ErrorTypes.InvalidAccount, errorResponse.Tipo);
    }

    [Fact]
    public async Task Handle_ContaInativa_DeveRetornarInactiveAccount()
    {
        // Arrange
        var request = new SaldoRequest
        {
            IdContaCorrente = "ContaInativa"
        };

        var contaInativa = new ContaCorrente
        {
            IdContaCorrente = "ContaInativa",
            Numero = 123,
            Nome = "Conta Inativa",
            Ativo = false
        };

        _saldoQueryStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns(contaInativa);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var errorResponse = result as ErrorResponse;
        Assert.NotNull(errorResponse);
        Assert.Equal(ErrorTypes.InactiveAccount, errorResponse.Tipo);
    }

    [Fact]
    public async Task Handle_SemMovimentos_DeveRetornarSaldoZero()
    {
        // Arrange
        var request = new SaldoRequest
        {
            IdContaCorrente = "ContaAtiva"
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "ContaAtiva",
            Numero = 123,
            Nome = "Conta Ativa",
            Ativo = true
        };

        _saldoQueryStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns(contaAtiva);
        _saldoQueryStore.ObterMovimentosPorContaCorrente(Arg.Any<string>()).Returns(new List<Movimento>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var response = result as SaldoResponse;
        Assert.NotNull(response);
        Assert.Equal(0, response.Saldo);
        Assert.Equal(contaAtiva.Numero, response.NumeroContaCorrente);
        Assert.Equal(contaAtiva.Nome, response.NomeTitular);
        Assert.NotNull(response.DataHoraConsulta);
    }

    [Fact]
    public async Task Handle_ComMovimentos_DeveRetornarSaldoCorreto()
    {
        // Arrange
        var request = new SaldoRequest
        {
            IdContaCorrente = "ContaAtiva"
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "ContaAtiva",
            Numero = 123,
            Nome = "Conta Ativa",
            Ativo = true
        };

        var movimentos = new List<Movimento>
        {
            new Movimento { IdMovimento = "1", IdContaCorrente = "ContaAtiva", TipoMovimento = TipoMovimento.Credito, Valor = 100m },
            new Movimento { IdMovimento = "2", IdContaCorrente = "ContaAtiva", TipoMovimento = TipoMovimento.Debito, Valor = 30m },
            new Movimento { IdMovimento = "3", IdContaCorrente = "ContaAtiva", TipoMovimento = TipoMovimento.Credito, Valor = 50m }
        };

        _saldoQueryStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns(contaAtiva);
        _saldoQueryStore.ObterMovimentosPorContaCorrente(Arg.Any<string>()).Returns(movimentos);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var response = result as SaldoResponse;
        Assert.NotNull(response);
        Assert.Equal(120m, response.Saldo); // 100 + 50 - 30 = 120
        Assert.Equal(contaAtiva.Numero, response.NumeroContaCorrente);
        Assert.Equal(contaAtiva.Nome, response.NomeTitular);
        Assert.NotNull(response.DataHoraConsulta);
    }
}