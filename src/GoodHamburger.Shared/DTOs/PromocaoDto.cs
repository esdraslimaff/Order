using GoodHamburger.Domain.Enums;

public class PromocaoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public decimal Percentual { get; set; }
    public bool Ativo { get; set; }
    public List<TipoItem> RequisitosTipo { get; set; } = new();
    public List<Guid> ItensObrigatoriosIds { get; set; } = new();
}