using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Tests.Entities
{
    public class PromocaoTests
    {

        private static Item CriarItem(string nome, decimal preco, TipoItem tipo, Guid? id = null)
        {
            if (id.HasValue)
                return new Item(id.Value, nome, preco, tipo);
            return new Item(nome, preco, tipo);
        }

        #region Construtor

        [Fact]
        public void Construtor_DeveCriarPromocaoAtivaComListasVazias()
        {
            var promocao = new Promocao("Combo Teste", 0.15m);

            promocao.Nome.Should().Be("Combo Teste");
            promocao.Percentual.Should().Be(0.15m);
            promocao.Ativo.Should().BeTrue();
            promocao.RequisitosTipo.Should().BeEmpty();
            promocao.ItensObrigatoriosIds.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Construtor_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            Action act = () => new Promocao(nomeInvalido, 0.10m);
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(-50)]
        [InlineData(1.1)]
        [InlineData(2)]
        public void Construtor_ComPercentualForaDoIntervalo_DeveLancarExcecao(decimal percentualInvalido)
        {
            Action act = () => new Promocao("Promo", percentualInvalido);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*percentual de desconto deve estar entre 0 e 1*");
        }

        #endregion

        #region AdicionarRequisitoTipo

        [Fact]
        public void AdicionarRequisitoTipo_DeveAdicionarTipo()
        {
            var promocao = new Promocao("Teste", 0.10m);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);

            promocao.RequisitosTipo.Should().ContainSingle()
                .Which.Should().Be(TipoItem.Sanduiche);
        }

        [Fact]
        public void AdicionarRequisitoTipo_NaoDeveAdicionarDuplicado()
        {
            var promocao = new Promocao("Teste", 0.10m);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);

            promocao.RequisitosTipo.Should().HaveCount(1);
        }

        #endregion

        #region AdicionarItemObrigatorio

        [Fact]
        public void AdicionarItemObrigatorio_DeveAdicionarId()
        {
            var promocao = new Promocao("Teste", 0.10m);
            var id = Guid.NewGuid();
            promocao.AdicionarItemObrigatorio(id);

            promocao.ItensObrigatoriosIds.Should().ContainSingle()
                .Which.Should().Be(id);
        }

        [Fact]
        public void AdicionarItemObrigatorio_NaoDeveDuplicar()
        {
            var promocao = new Promocao("Teste", 0.10m);
            var id = Guid.NewGuid();
            promocao.AdicionarItemObrigatorio(id);
            promocao.AdicionarItemObrigatorio(id);

            promocao.ItensObrigatoriosIds.Should().HaveCount(1);
        }

        #endregion

        #region AlternarStatus

        [Fact]
        public void AlternarStatus_DeveInverterAtivo()
        {
            var promocao = new Promocao("Teste", 0.10m);
            promocao.Ativo.Should().BeTrue();

            promocao.AlternarStatus();
            promocao.Ativo.Should().BeFalse();

            promocao.AlternarStatus();
            promocao.Ativo.Should().BeTrue();
        }

        #endregion

        #region PodeSerAplicada

        private List<PedidoItem> CriarItensPedido(params (Guid id, TipoItem tipo)[] itens)
        {
            return itens.Select(i => new PedidoItem(
                CriarItem("Nome", 1m, i.tipo, i.id),
                quantidade: 1
            )).ToList();
        }

        [Fact]
        public void PodeSerAplicada_ComApenasRequisitosTipo_DeveRetornarTrueQuandoTodosTiposPresentes()
        {
            var promocao = new Promocao("Combo", 0.20m);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);
            promocao.AdicionarRequisitoTipo(TipoItem.Bebida);

            var itens = CriarItensPedido(
                (Guid.NewGuid(), TipoItem.Sanduiche),
                (Guid.NewGuid(), TipoItem.Bebida)
            );

            promocao.PodeSerAplicada(itens).Should().BeTrue();
        }

        [Fact]
        public void PodeSerAplicada_ComApenasRequisitosTipo_DeveRetornarFalseQuandoFaltaTipo()
        {
            var promocao = new Promocao("Combo", 0.20m);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);
            promocao.AdicionarRequisitoTipo(TipoItem.Bebida);

            var itens = CriarItensPedido(
                (Guid.NewGuid(), TipoItem.Sanduiche)
            );

            promocao.PodeSerAplicada(itens).Should().BeFalse();
        }

        [Fact]
        public void PodeSerAplicada_ComApenasItensObrigatorios_DeveRetornarTrueQuandoTodosIdsPresentes()
        {
            var promocao = new Promocao("Especial", 0.15m);
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            promocao.AdicionarItemObrigatorio(id1);
            promocao.AdicionarItemObrigatorio(id2);

            var itens = CriarItensPedido(
                (id1, TipoItem.Sanduiche),
                (id2, TipoItem.Bebida)
            );

            promocao.PodeSerAplicada(itens).Should().BeTrue();
        }

        [Fact]
        public void PodeSerAplicada_ComApenasItensObrigatorios_DeveRetornarFalseQuandoFaltaId()
        {
            var promocao = new Promocao("Especial", 0.15m);
            promocao.AdicionarItemObrigatorio(Guid.NewGuid());
            promocao.AdicionarItemObrigatorio(Guid.NewGuid());

            var itens = CriarItensPedido(
                (Guid.NewGuid(), TipoItem.Sanduiche)
            );

            promocao.PodeSerAplicada(itens).Should().BeFalse();
        }

        [Fact]
        public void PodeSerAplicada_ComAmbosRequisitos_DeveRetornarTrueQuandoTodosAtendidos()
        {
            var promocao = new Promocao("Combo Específico", 0.25m);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);
            promocao.AdicionarRequisitoTipo(TipoItem.Bebida);
            var idSanduiche = Guid.NewGuid();
            promocao.AdicionarItemObrigatorio(idSanduiche);

            var itens = CriarItensPedido(
                (idSanduiche, TipoItem.Sanduiche),
                (Guid.NewGuid(), TipoItem.Bebida)
            );

            promocao.PodeSerAplicada(itens).Should().BeTrue();
        }

        [Fact]
        public void PodeSerAplicada_ComAmbosRequisitos_DeveRetornarFalseQuandoFaltaItemObrigatorio()
        {
            var promocao = new Promocao("Combo Específico", 0.25m);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);
            promocao.AdicionarRequisitoTipo(TipoItem.Bebida);
            promocao.AdicionarItemObrigatorio(Guid.NewGuid());

            var itens = CriarItensPedido(
                (Guid.NewGuid(), TipoItem.Sanduiche),
                (Guid.NewGuid(), TipoItem.Bebida)
            );

            promocao.PodeSerAplicada(itens).Should().BeFalse();
        }

        [Fact]
        public void PodeSerAplicada_SemRequisitos_DeveRetornarFalse()
        {
            var promocao = new Promocao("Vazia", 0.10m);
            var itens = CriarItensPedido((Guid.NewGuid(), TipoItem.Sanduiche));

            promocao.PodeSerAplicada(itens).Should().BeFalse();
        }

        [Fact]
        public void PodeSerAplicada_PromocaoInativa_DeveRetornarFalse()
        {
            var promocao = new Promocao("Inativa", 0.10m);
            promocao.AlternarStatus();
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);

            var itens = CriarItensPedido((Guid.NewGuid(), TipoItem.Sanduiche));

            promocao.PodeSerAplicada(itens).Should().BeFalse();
        }

        #endregion
    }
}