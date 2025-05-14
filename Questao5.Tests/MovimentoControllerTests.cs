using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Infrastructure.Services.Controllers;
using MediatR;

namespace Questao5.Tests;

public class MovimentoControllerTests
{
    private readonly IMediator _mediator;
    private readonly MovimentoController _controller;

    public MovimentoControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new MovimentoController(_mediator);
    }

    [Fact]
    public async Task RealizarMovimento_ComSucesso_DeveRetornarOk()
    {
        // Arrange
        var request = new MovimentoRequest
        {
            IdRequisicao = Guid.NewGuid().ToString(),
            IdContaCorrente = "ContaAtiva",
            Valor = 100,
            TipoMovimento = "C"
        };

        var response = new MovimentoResponse
        {
            IdMovimento = Guid.NewGuid().ToString()
        };

        _mediator.Send(Arg.Any<MovimentoRequest>()).Returns(response);

        // Act
        var result = await _controller.RealizarMovimento(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var movimentoResponse = okResult.Value as MovimentoResponse;
        Assert.NotNull(movimentoResponse);
        Assert.Equal(response.IdMovimento, movimentoResponse.IdMovimento);
    }

    [Fact]
    public async Task RealizarMovimento_ComErro_DeveRetornarBadRequest()
    {
        // Arrange
        var request = new MovimentoRequest
        {
            IdRequisicao = Guid.NewGuid().ToString(),
            IdContaCorrente = "ContaInvalida",
            Valor = 100,
            TipoMovimento = "C"
        };

        var errorResponse = new ErrorResponse
        {
            Tipo = "INVALID_ACCOUNT",
            Mensagem = "Conta corrente não encontrada."
        };

        _mediator.Send(Arg.Any<MovimentoRequest>()).Returns(errorResponse);

        // Act
        var result = await _controller.RealizarMovimento(request);

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