namespace GoodHamburger.Domain.Entities
{
    public class GrupoOpcao : BaseEntity
    {
        public string Nome { get; private set; }
        public TipoSelecao TipoSelecao { get; private set; }
        public bool Obrigatorio { get; private set; }
        public int? MinimoSelecoes { get; private set; }
        public int? MaximoSelecoes { get; private set; }

        private readonly List<Opcao> _opcoes = new();
        public IReadOnlyCollection<Opcao> Opcoes => _opcoes.AsReadOnly();

        private GrupoOpcao() { }

        public GrupoOpcao(string nome, TipoSelecao tipo, bool obrigatorio)
        {
            Nome = nome;
            TipoSelecao = tipo;
            Obrigatorio = obrigatorio;
        }

        public void AdicionarOpcao(string nome, decimal precoAdicional)
        {
            var opcao = new Opcao(nome, precoAdicional);
            opcao.SetGrupo(this);
            _opcoes.Add(opcao);
        }
    }

    public enum TipoSelecao
    {
        Unica = 1,
        Multipla = 2
    }
}