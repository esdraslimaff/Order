namespace GoodHamburger.Domain.Entities
{
    public class Opcao : BaseEntity
    {
        public string Nome { get; private set; }
        public decimal PrecoAdicional { get; private set; }
        public Guid GrupoOpcaoId { get; private set; }
        public GrupoOpcao GrupoOpcao { get; private set; } = null!;

        private Opcao() { }

        internal Opcao(string nome, decimal precoAdicional) : this()
        {
            Nome = nome;
            PrecoAdicional = precoAdicional;
        }

        internal void SetGrupo(GrupoOpcao grupo)
        {
            GrupoOpcao = grupo;
            GrupoOpcaoId = grupo.Id;
        }
    }
}