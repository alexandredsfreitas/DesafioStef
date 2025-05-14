using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Questao5.Application.Queries.Requests;

public class SaldoRequest : IRequest<object>
{
    [Required(ErrorMessage = "A identificação da conta corrente é obrigatória")]
    public string IdContaCorrente { get; set; }
}