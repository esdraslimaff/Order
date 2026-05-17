using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities
{
    public class PedidoItem : BaseEntity
    {
        public Guid PedidoId { get; private set; }
        public Guid ProdutoId { get; private set; }
        public string Nome { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public TipoItem Tipo { get; private set; }
        public int Quantidade { get; private set; }
        public string? Observacao { get; private set; }

        private readonly List<PedidoItemOpcao> _opcoesSelecionadas = new();
        public IReadOnlyCollection<PedidoItemOpcao> OpcoesSelecionadas => _opcoesSelecionadas.AsReadOnly();

        protected PedidoItem() { }

        public PedidoItem(Item produto, int quantidade, string? observacao = null)
        {
            ProdutoId = produto.Id;
            Nome = produto.Nome;
            PrecoUnitario = produto.PrecoUnitario;
            Tipo = produto.Tipo;
            Quantidade = quantidade;
            Observacao = observacao;
        }

        public void AdicionarOpcao(Opcao opcao, GrupoOpcao grupo, int quantidade = 1)
        {
            _opcoesSelecionadas.Add(new PedidoItemOpcao(opcao, grupo, quantidade));
        }

        public void RemoverOpcao(Guid opcaoId)
        {
            var opcao = _opcoesSelecionadas.FirstOrDefault(o => o.Id == opcaoId);
            if (opcao != null) _opcoesSelecionadas.Remove(opcao);
        }

        public decimal PrecoTotal => (PrecoUnitario * Quantidade) + _opcoesSelecionadas.Sum(o => o.PrecoTotal);
    }
}