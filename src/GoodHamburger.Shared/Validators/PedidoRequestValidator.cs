using FluentValidation;
using GoodHamburger.Shared.DTOs;

namespace GoodHamburger.Shared.Validators
{
    public class PedidoRequestValidator : AbstractValidator<PedidoRequest>
    {
        public PedidoRequestValidator()
        {
            RuleFor(x => x.Itens)
                .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");

            RuleForEach(x => x.Itens)
                .SetValidator(new ItemPedidoRequestValidator());
        }
    }
}