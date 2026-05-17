using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using System.Linq;

public class Promocao : BaseEntity
{
    public string Nome { get; private set; }
    public decimal Percentual { get; private set; }
    public bool Ativo { get; private set; }
    public List<TipoItem> RequisitosTipo { get; private set; } = new();
    public List<Guid> ItensObrigatoriosIds { get; private set; } = new();
    public Promocao(string nome, decimal percentual)
    {
        Nome = nome;
        Percentual = percentual;
        Ativo = true;
    }

    public void AlternarStatus()
    {
        Ativo = !Ativo;
    }

    public void AdicionarRequisitoTipo(TipoItem tipo) => RequisitosTipo.Add(tipo);
    public void AdicionarItemObrigatorio(Guid itemId) => ItensObrigatoriosIds.Add(itemId);

    /// <summary>
    /// Verifica se esta promoção se aplica à coleção de itens do pedido.
    /// Ambos os critérios (tipo e itens obrigatórios) são considerados,
    /// mas apenas se estiverem preenchidos.
    /// </summary>
    public bool PodeSerAplicada(IReadOnlyCollection<PedidoItem> itensPedido)
    {
        if (!Ativo) return false;

        if (ItensObrigatoriosIds.Any())
        {
            var idsPresentes = itensPedido.Select(i => i.ProdutoId).ToHashSet();
            if (!ItensObrigatoriosIds.All(id => idsPresentes.Contains(id)))
                return false;
        }

        if (RequisitosTipo.Any())
        {
            var tiposPresentes = itensPedido.Select(i => i.Tipo).Distinct().ToHashSet();
            if (!RequisitosTipo.All(t => tiposPresentes.Contains(t)))
                return false;
        }

        return true;
    }
}