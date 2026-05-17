using FluentValidation;
using GoodHamburger.Shared.DTOs;

namespace GoodHamburger.Shared.Validators
{
    public class ItemPedidoRequestValidator : AbstractValidator<ItemPedidoRequest>
    {
        public ItemPedidoRequestValidator()
        {
            RuleFor(i => i.Quantidade)
                .GreaterThan(0)
                .WithMessage("A quantidade de cada item deve ser pelo menos 1.");

            RuleFor(i => i.OpcoesSelecionadas)
                .Must(opcoes => opcoes == null || opcoes.Select(o => o.OpcaoId).Distinct().Count() == opcoes.Count)
                .WithMessage("Não é permitido selecionar a mesma opção mais de uma vez.");

            RuleForEach(i => i.OpcoesSelecionadas)
                .SetValidator(new OpcaoSelecionadaRequestValidator())
                .When(i => i.OpcoesSelecionadas != null && i.OpcoesSelecionadas.Any());
        }
    }
}