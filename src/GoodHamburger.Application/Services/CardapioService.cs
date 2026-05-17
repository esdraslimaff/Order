using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Shared.DTOs;

public class CardapioService : ICardapioService
{
    private readonly IItemRepository _itemRepository;

    public CardapioService(IItemRepository itemRepository) => _itemRepository = itemRepository;

    public async Task<IEnumerable<ItemCardapioDto>> ObterItensAsync()
    {
        var itens = await _itemRepository.GetAllWithGruposAsync();
        return itens.Select(i => new ItemCardapioDto
        {
            Id = i.Id,
            Nome = i.Nome,
            PrecoUnitario = i.PrecoUnitario,
            Tipo = i.Tipo,
            GruposOpcoes = i.GruposOpcoes.Select(ig => new GrupoOpcaoDto
            {
                Id = ig.GrupoOpcao.Id,
                Nome = ig.GrupoOpcao.Nome,
                TipoSelecao = ig.GrupoOpcao.TipoSelecao,
                Obrigatorio = ig.Obrigatorio,
                MinimoSelecoes = ig.MinimoSelecoes,
                MaximoSelecoes = ig.MaximoSelecoes,
                Opcoes = ig.GrupoOpcao.Opcoes.Select(o => new OpcaoDto
                {
                    Id = o.Id,
                    Nome = o.Nome,
                    PrecoAdicional = o.PrecoAdicional
                }).ToList()
            }).ToList()
        });
    }
}