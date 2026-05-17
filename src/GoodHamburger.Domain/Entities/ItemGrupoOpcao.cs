namespace GoodHamburger.Domain.Entities
{
    public class ItemGrupoOpcao : BaseEntity
    {
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; } = null!;
        public Guid GrupoOpcaoId { get; private set; }
        public GrupoOpcao GrupoOpcao { get; private set; } = null!;
        public bool Obrigatorio { get; private set; }
        public int? MinimoSelecoes { get; private set; }
        public int? MaximoSelecoes { get; private set; }

        private ItemGrupoOpcao() { }

        internal ItemGrupoOpcao(Item item, GrupoOpcao grupo, bool obrigatorio, int? min, int? max)
        {
            Item = item;
            GrupoOpcao = grupo;
            Obrigatorio = obrigatorio;
            MinimoSelecoes = min;
            MaximoSelecoes = max;
        }
    }
}