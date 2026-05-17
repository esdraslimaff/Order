namespace GoodHamburger.Shared.DTOs
{
    public class PedidoResponse
    {
        public Guid Id { get; set; }
        public Guid? PromocaoId { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<PedidoItemResponse> Itens { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal DescontoPercentual { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal TotalFinal { get; set; }
        public string? Observacao { get; set; }
    }

    public class PedidoItemResponse
    {
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public string? Observacao { get; set; }
        public List<OpcaoSelecionadaResponse> OpcoesSelecionadas { get; set; } = new();
    }

    public class OpcaoSelecionadaResponse
    {
        public Guid Id { get; set; }
        public string NomeOpcao { get; set; }
        public string NomeGrupo { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
    }
}