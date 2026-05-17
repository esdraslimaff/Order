using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities
{
    public class Pedido : BaseEntity
    {
        private readonly List<PedidoItem> _itens = new();
        public IReadOnlyCollection<PedidoItem> Itens => _itens.AsReadOnly();
        public decimal Subtotal { get; private set; }
        public decimal DescontoPercentual { get; private set; }
        public decimal ValorDesconto { get; private set; }
        public decimal TotalFinal { get; private set; }
        public Guid? PromocaoId { get; private set; }
        public string? ObservacaoGeral { get; set; }

        public void AdicionarItem(Item produto, int quantidade = 1, string? observacao = null)
        {
            _itens.Add(new PedidoItem(produto, quantidade, observacao));
            RecalcularTotais();
        }

        public void DefinirObservacao(string observacao) => ObservacaoGeral = observacao;

        public void RemoverItem(Guid pedidoItemId)
        {
            var item = _itens.FirstOrDefault(i => i.Id == pedidoItemId)
                ?? throw new DomainException("Item não encontrado no pedido.");
            _itens.Remove(item);
            RecalcularTotais();
        }

        public void AplicarPromocoes(IEnumerable<Promocao> promocoesDisponiveis)
        {
            var melhor = promocoesDisponiveis
                .Where(p => p.PodeSerAplicada(_itens))
                .MaxBy(p => p.Percentual);

            if (melhor != null)
            {
                DescontoPercentual = melhor.Percentual;
                PromocaoId = melhor.Id;
            }
            else
            {
                DescontoPercentual = 0;
                PromocaoId = null;
            }
            RecalcularTotais();
        }

        private void RecalcularTotais()
        {
            Subtotal = _itens.Sum(i => i.PrecoTotal);
            ValorDesconto = Subtotal * DescontoPercentual;
            TotalFinal = Subtotal - ValorDesconto;
            RegistrarAlteracao();
        }

        public PedidoItem? ObterItem(Guid pedidoItemId) => _itens.FirstOrDefault(i => i.Id == pedidoItemId);

        public void LimparItens()
        {
            while (_itens.Any())
            {
                var item = _itens.First();
                _itens.Remove(item);
            }
            RecalcularTotais();
        }
    }
}