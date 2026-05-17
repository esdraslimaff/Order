using FluentValidation;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Domain.Interfaces.Repository;
using GoodHamburger.Shared.DTOs;
using Mapster;

namespace GoodHamburger.Application.Services
{
    public class PedidoAppService : IPedidoAppService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IPromocaoRepository _promocaoRepository;
        private readonly IValidator<PedidoRequest> _validator;

        public PedidoAppService(
            IPedidoRepository pedidoRepository,
            IItemRepository itemRepository,
            IPromocaoRepository promocaoRepository,
            IValidator<PedidoRequest> validator)
        {
            _pedidoRepository = pedidoRepository;
            _itemRepository = itemRepository;
            _promocaoRepository = promocaoRepository;
            _validator = validator;
        }

        public async Task<PedidoResponse> CriarPedidoAsync(PedidoRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new DomainException(validationResult.Errors.First().ErrorMessage);

            if (request.Itens == null || !request.Itens.Any())
                throw new DomainException("Nenhum item informado.");

            var novoPedido = new Pedido();
            if (!string.IsNullOrWhiteSpace(request.Observacao))
                novoPedido.DefinirObservacao(request.Observacao);

            foreach (var itemReq in request.Itens)
            {
                var produto = await _itemRepository.GetByIdWithGruposOpcoesAsync(itemReq.ItemId)
                    ?? throw new DomainException($"Item {itemReq.ItemId} não encontrado.");

                novoPedido.AdicionarItem(produto, itemReq.Quantidade, itemReq.Observacao);
                var pedidoItem = novoPedido.Itens.First(i => i.ProdutoId == produto.Id);

                foreach (var opc in itemReq.OpcoesSelecionadas)
                {
                    var grupo = produto.GruposOpcoes
                        .Select(ig => ig.GrupoOpcao)
                        .FirstOrDefault(g => g.Opcoes.Any(o => o.Id == opc.OpcaoId));

                    if (grupo == null)
                        throw new DomainException($"A opção {opc.OpcaoId} não está disponível para o item {produto.Nome}.");

                    var opcao = grupo.Opcoes.First(o => o.Id == opc.OpcaoId);
                    pedidoItem.AdicionarOpcao(opcao, grupo, opc.Quantidade);
                }
            }

            var promocoes = await _promocaoRepository.ObterTodasAtivasAsync();
            novoPedido.AplicarPromocoes(promocoes);

            await _pedidoRepository.AddAsync(novoPedido);
            return novoPedido.Adapt<PedidoResponse>();
        }

        public async Task<IEnumerable<PedidoResponse>> ListarTodosAsync()
        {
            var pedidos = await _pedidoRepository.GetAllAsync();
            return pedidos.Adapt<IEnumerable<PedidoResponse>>();
        }

        public async Task<PedidoResponse?> ObterPorIdAsync(Guid id)
        {
            var pedido = await _pedidoRepository.GetPedidoComItensAsync(id);
            return pedido?.Adapt<PedidoResponse>();
        }

        public async Task RemoverAsync(Guid id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null) throw new DomainException("Pedido não encontrado.");
            await _pedidoRepository.DeleteAsync(pedido);
        }

        public async Task AtualizarPedidoAsync(Guid id, PedidoRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new DomainException(validationResult.Errors.First().ErrorMessage);

            var pedido = await _pedidoRepository.GetPedidoComItensAsync(id)
                ?? throw new DomainException("Pedido não encontrado.");

            pedido.LimparItens();
            await _pedidoRepository.SaveChangesAsync();

            foreach (var itemReq in request.Itens)
            {
                var produto = await _itemRepository.GetByIdWithGruposOpcoesAsync(itemReq.ItemId)
                    ?? throw new DomainException($"Item {itemReq.ItemId} não encontrado.");

                pedido.AdicionarItem(produto, itemReq.Quantidade, itemReq.Observacao);
                var pedidoItem = pedido.Itens.First(i => i.ProdutoId == produto.Id);

                foreach (var opc in itemReq.OpcoesSelecionadas ?? new())
                {
                    var grupo = produto.GruposOpcoes
                        .Select(ig => ig.GrupoOpcao)
                        .FirstOrDefault(g => g.Opcoes.Any(o => o.Id == opc.OpcaoId));

                    if (grupo == null)
                        throw new DomainException($"A opção {opc.OpcaoId} não está disponível para o item {produto.Nome}.");

                    var opcao = grupo.Opcoes.First(o => o.Id == opc.OpcaoId);
                    pedidoItem.AdicionarOpcao(opcao, grupo, opc.Quantidade);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Observacao))
                pedido.DefinirObservacao(request.Observacao);

            var promocoes = await _promocaoRepository.ObterTodasAtivasAsync();
            pedido.AplicarPromocoes(promocoes);

            await _pedidoRepository.SaveChangesAsync();
        }
    }
}