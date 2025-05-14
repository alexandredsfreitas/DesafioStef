using Microsoft.Data.Sqlite;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using System.Text.Json;

namespace Questao5.Tests;

public class MovimentoHandlerTests
{
    private readonly IMovimentoCommandStore _movimentoCommandStore;
    private readonly MovimentoHandler _handler;

    public MovimentoHandlerTests()
    {
        _movimentoCommandStore = Substitute.For<IMovimentoCommandStore>();
        _handler = new MovimentoHandler(_movimentoCommandStore);
    }

    [Fact]
    public async Task Handle_ContaInexistente_DeveRetornarInvalidAccount()
    {
        // Arrange
        var request = new MovimentoRequest
        {
            IdRequisicao = Guid.NewGuid().ToString(),
            IdContaCorrente = "ContaInexistente",
            Valor = 100,
            TipoMovimento = TipoMovimento.Credito
        };

        _movimentoCommandStore.ObterIdempotenciaPorChave(Arg.Any<string>()).Returns((Idempotencia)null);
        _movimentoCommandStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns((ContaCorrente)null);

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
        var request = new MovimentoRequest
        {
            IdRequisicao = Guid.NewGuid().ToString(),
            IdContaCorrente = "ContaInativa",
            Valor = 100,
            TipoMovimento = TipoMovimento.Credito
        };

        var contaInativa = new ContaCorrente
        {
            IdContaCorrente = "ContaInativa",
            Numero = 123,
            Nome = "Conta Inativa",
            Ativo = false
        };

        _movimentoCommandStore.ObterIdempotenciaPorChave(Arg.Any<string>()).Returns((Idempotencia)null);
        _movimentoCommandStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns(contaInativa);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var errorResponse = result as ErrorResponse;
        Assert.NotNull(errorResponse);
        Assert.Equal(ErrorTypes.InactiveAccount, errorResponse.Tipo);
    }

    [Fact]
    public async Task Handle_ValorZero_DeveRetornarInvalidValue()
    {
        // Arrange
        var request = new MovimentoRequest
        {
            IdRequisicao = Guid.NewGuid().ToString(),
            IdContaCorrente = "ContaAtiva",
            Valor = 0,
            TipoMovimento = TipoMovimento.Credito
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "ContaAtiva",
            Numero = 123,
            Nome = "Conta Ativa",
            Ativo = true
        };

        _movimentoCommandStore.ObterIdempotenciaPorChave(Arg.Any<string>()).Returns((Idempotencia)null);
        _movimentoCommandStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns(contaAtiva);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var errorResponse = result as ErrorResponse;
        Assert.NotNull(errorResponse);
        Assert.Equal(ErrorTypes.InvalidValue, errorResponse.Tipo);
    }

    [Fact]
    public async Task Handle_TipoMovimentoInvalido_DeveRetornarInvalidType()
    {
        // Arrange
        var request = new MovimentoRequest
        {
            IdRequisicao = Guid.NewGuid().ToString(),
            IdContaCorrente = "ContaAtiva",
            Valor = 100,
            TipoMovimento = "X" // Tipo inválido
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "ContaAtiva",
            Numero = 123,
            Nome = "Conta Ativa",
            Ativo = true
        };

        _movimentoCommandStore.ObterIdempotenciaPorChave(Arg.Any<string>()).Returns((Idempotencia)null);
        _movimentoCommandStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns(contaAtiva);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var errorResponse = result as ErrorResponse;
        Assert.NotNull(errorResponse);
        Assert.Equal(ErrorTypes.InvalidType, errorResponse.Tipo);
    }

    [Fact]
    public async Task Handle_MovimentoValido_DeveRetornarIdMovimento()
    {
        // Arrange
        var request = new MovimentoRequest
        {
            IdRequisicao = Guid.NewGuid().ToString(),
            IdContaCorrente = "ContaAtiva",
            Valor = 100,
            TipoMovimento = TipoMovimento.Credito
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "ContaAtiva",
            Numero = 123,
            Nome = "Conta Ativa",
            Ativo = true
        };

        _movimentoCommandStore.ObterIdempotenciaPorChave(Arg.Any<string>()).Returns((Idempotencia)null);
        _movimentoCommandStore.ObterContaCorrentePorId(Arg.Any<string>()).Returns(contaAtiva);
        _movimentoCommandStore.InserirMovimento(Arg.Any<Movimento>()).Returns(Task.FromResult(Guid.NewGuid().ToString()));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var response = result as MovimentoResponse;
        Assert.NotNull(response);
        Assert.NotNull(response.IdMovimento);
        Assert.NotEmpty(response.IdMovimento);
    }

    [Fact]
    public async Task Handle_RequisicaoIdempotente_DeveRetornarResultadoAnterior()
    {
        // Arrange
        var idRequisicao = Guid.NewGuid().ToString();
        var idMovimentoAnterior = Guid.NewGuid().ToString();

        var request = new MovimentoRequest
        {
            IdRequisicao = idRequisicao,
            IdContaCorrente = "ContaAtiva",
            Valor = 100,
            TipoMovimento = TipoMovimento.Credito
        };

        var resultadoAnterior = new MovimentoResponse
        {
            IdMovimento = idMovimentoAnterior
        };

        var idempotencia = new Idempotencia
        {
            ChaveIdempotencia = idRequisicao,
            Requisicao = JsonSerializer.Serialize(request),
            Resultado = JsonSerializer.Serialize(resultadoAnterior)
        };

        _movimentoCommandStore.ObterIdempotenciaPorChave(idRequisicao).Returns(idempotencia);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var response = result as MovimentoResponse;
        Assert.NotNull(response);
        Assert.Equal(idMovimentoAnterior, response.IdMovimento);

        // Verificar que não houve nova inserção
        await _movimentoCommandStore.DidNotReceive().InserirMovimento(Arg.Any<Movimento>());
    }
}