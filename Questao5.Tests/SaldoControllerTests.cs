using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Questao5.Application.Commands.Responses;
using Questao5.Infrastructure.Services.Controllers;
using MediatR;

namespace Questao5.Tests;

public class SaldoControllerTests
{
    private readonly IMediator _mediator;
    private readonly SaldoController _controller;

    public SaldoControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new SaldoController(_mediator);
    }

    [Fact]
    public async Task ConsultarSaldo_ComSucesso_DeveRetornarOk()
    {
        // Arrange
        var idContaCorrente = "ContaAtiva";
        var response = new Application.Queries.Responses.SaldoResponse
        {
            NumeroContaCorrente = 123,
            NomeTitular = "Conta Ativa",
            DataHoraConsulta = DateTime.Now.ToString(),
            Saldo = 100
        };

        _mediator.Send(Arg.Any<Application.Queries.Requests.SaldoRequest>()).Returns(response);

        // Act
        var result = await _controller.ConsultarSaldo(idContaCorrente);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var saldoResponse = okResult.Value as Application.Queries.Responses.SaldoResponse;
        Assert.NotNull(saldoResponse);
        Assert.Equal(response.NumeroContaCorrente, saldoResponse.NumeroContaCorrente);
        Assert.Equal(response.NomeTitular, saldoResponse.NomeTitular);
        Assert.Equal(response.Saldo, saldoResponse.Saldo);
    }

    [Fact]
    public async Task ConsultarSaldo_ComErro_DeveRetornarBadRequest()
    {
        // Arrange
        var idContaCorrente = "ContaInvalida";
        var errorResponse = new ErrorResponse
        {
            Tipo = "INVALID_ACCOUNT",
            Mensagem = "Conta corrente não encontrada."
        };

        _mediator.Send(Arg.Any<Application.Queries.Requests.SaldoRequest>()).Returns(errorResponse);

        // Act
        var result = await _controller.ConsultarSaldo(idContaCorrente);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);

        var error = badRequestResult.Value as ErrorResponse;
        Assert.NotNull(error);
        Assert.Equal(errorResponse.Tipo, error.Tipo);
        Assert.Equal(errorResponse.Mensagem, error.Mensagem);
    }
}