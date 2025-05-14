using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using System.Net;
namespace Questao5.Infrastructure.Services.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SaldoController : ControllerBase
{
    private readonly IMediator _mediator;

    public SaldoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Consulta o saldo de uma conta corrente
    /// </summary>
    /// <param name="idContaCorrente">Identificação da conta corrente</param>
    /// <returns>Saldo da conta corrente</returns>
    /// <response code="200">Consulta realizada com sucesso</response>
    /// <response code="400">Dados inválidos de entrada</response>
    [HttpGet("{idContaCorrente}")]
    [ProducesResponseType(typeof(Application.Queries.Responses.SaldoResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ConsultarSaldo(string idContaCorrente)
    {
        var request = new Application.Queries.Requests.SaldoRequest { IdContaCorrente = idContaCorrente };
        var response = await _mediator.Send(request);

        if (response is ErrorResponse)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}