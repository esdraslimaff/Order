namespace GoodHamburger.Shared.DTOs
{
    public class PedidoRequest
    {
        public List<ItemPedidoRequest> Itens { get; set; } = new();
        public string? Observacao { get; set; }
    }

    public class ItemPedidoRequest
    {
        public Guid ItemId { get; set; }
        public int Quantidade { get; set; } = 1;
        public List<OpcaoSelecionadaRequest> OpcoesSelecionadas { get; set; } = new();
        public string? Observacao { get; set; }
    }

    public class OpcaoSelecionadaRequest
    {
        public Guid OpcaoId { get; set; }
        public int Quantidade { get; set; } = 1;
    }
}