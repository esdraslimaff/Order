using FluentValidation;
using GoodHamburger.Shared.DTOs;

namespace GoodHamburger.Shared.Validators
{
    public class OpcaoSelecionadaRequestValidator : AbstractValidator<OpcaoSelecionadaRequest>
    {
        public OpcaoSelecionadaRequestValidator()
        {
            RuleFor(o => o.Quantidade)
                .GreaterThan(0)
                .WithMessage("A quantidade de uma opção deve ser pelo menos 1.");
        }
    }
}