namespace GoodHamburger.Domain.Entities
{
    public class PedidoItemOpcao : BaseEntity
    {
        public Guid PedidoItemId { get; private set; }
        public Guid OpcaoId { get; private set; }
        public string NomeOpcao { get; private set; }
        public string NomeGrupo { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public int Quantidade { get; private set; }

        private PedidoItemOpcao() { }

        public PedidoItemOpcao(Opcao opcao, GrupoOpcao grupo, int quantidade = 1)
        {
            OpcaoId = opcao.Id;
            NomeOpcao = opcao.Nome;
            NomeGrupo = grupo.Nome;
            PrecoUnitario = opcao.PrecoAdicional;
            Quantidade = quantidade;
        }

        public decimal PrecoTotal => PrecoUnitario * Quantidade;
    }
}