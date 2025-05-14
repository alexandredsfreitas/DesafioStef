using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Questao5.Application.Commands.Requests;

public class MovimentoRequest : IRequest<object>
{
    [Required(ErrorMessage = "A identificação da requisição é obrigatória")]
    public string IdRequisicao { get; set; }

    [Required(ErrorMessage = "A identificação da conta corrente é obrigatória")]
    public string IdContaCorrente { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O tipo de movimento é obrigatório")]
    [StringLength(1, ErrorMessage = "O tipo de movimento deve ter 1 caractere")]
    [RegularExpression("[CD]", ErrorMessage = "O tipo de movimento deve ser 'C' para crédito ou 'D' para débito")]
    public string TipoMovimento { get; set; }
}