using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities
{
    public class Item : BaseEntity
    {
        public string Nome { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public TipoItem Tipo { get; private set; }

        private readonly List<ItemGrupoOpcao> _gruposOpcoes = new();
        public IReadOnlyCollection<ItemGrupoOpcao> GruposOpcoes => _gruposOpcoes.AsReadOnly();

        protected Item() { }

        public Item(string nome, decimal precoUnitario, TipoItem tipo)
        {
            Nome = nome;
            PrecoUnitario = precoUnitario;
            Tipo = tipo;
        }

        public Item(Guid id, string nome, decimal precoUnitario, TipoItem tipo) : this(nome, precoUnitario, tipo)
        {
            Id = id;
        }

        public void AdicionarGrupoOpcao(GrupoOpcao grupo, bool obrigatorio = false, int? min = null, int? max = null)
        {
            _gruposOpcoes.Add(new ItemGrupoOpcao(this, grupo, obrigatorio, min, max));
        }
    }
}