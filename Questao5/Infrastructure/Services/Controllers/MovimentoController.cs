using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using System.Net;

namespace Questao5.Infrastructure.Services.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MovimentoController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovimentoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Realiza uma movimentação na conta corrente
    /// </summary>
    /// <param name="request">Dados da movimentação</param>
    /// <returns>Identificação do movimento gerado</returns>
    /// <response code="200">Movimentação realizada com sucesso</response>
    /// <response code="400">Dados inválidos de entrada</response>
    [HttpPost]
    [ProducesResponseType(typeof(MovimentoResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> RealizarMovimento([FromBody] MovimentoRequest request)
    {
        var response = await _mediator.Send(request);

        if (response is ErrorResponse)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}