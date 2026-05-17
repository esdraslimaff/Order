using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Tests.Entities
{
    public class PedidoTests
    {
        private static Item CriarItem(string nome, decimal preco, TipoItem tipo, Guid? id = null)
        {
            if (id.HasValue)
                return new Item(id.Value, nome, preco, tipo);
            return new Item(nome, preco, tipo);
        }

        private static Promocao CriarPromocaoTipos(string nome, decimal percentual, params TipoItem[] tipos)
        {
            var promo = new Promocao(nome, percentual);
            foreach (var t in tipos) promo.AdicionarRequisitoTipo(t);
            return promo;
        }

        private static Promocao CriarPromocaoItens(string nome, decimal percentual, params Guid[] ids)
        {
            var promo = new Promocao(nome, percentual);
            foreach (var id in ids) promo.AdicionarItemObrigatorio(id);
            return promo;
        }

        #region Construtor

        [Fact]
        public void Construtor_DeveInicializarListaDeItensComoVazia()
        {
            var pedido = new Pedido();
            pedido.Itens.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void Construtor_DeveInicializarValoresFinanceirosZerados()
        {
            var pedido = new Pedido();
            pedido.Subtotal.Should().Be(0m);
            pedido.DescontoPercentual.Should().Be(0m);
            pedido.ValorDesconto.Should().Be(0m);
            pedido.TotalFinal.Should().Be(0m);
        }

        [Fact]
        public void Construtor_DeveInicializarSemPromocaoVinculada()
        {
            var pedido = new Pedido();
            pedido.PromocaoId.Should().BeNull();
        }

        [Fact]
        public void Construtor_DeveInicializarIdEDataDeCriacao()
        {
            var pedido = new Pedido();
            pedido.Id.Should().NotBeEmpty();
            pedido.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        #endregion

        #region AdicionarItem

        [Fact]
        public void AdicionarItem_DeveAdicionarItemERecalcularTotais()
        {
            var pedido = new Pedido();
            var item = CriarItem("X Burger", 5.00m, TipoItem.Sanduiche);
            pedido.AdicionarItem(item);

            pedido.Itens.Should().HaveCount(1);
            pedido.Subtotal.Should().Be(5.00m);
            pedido.TotalFinal.Should().Be(5.00m);
        }

        [Fact]
        public void AdicionarItem_ComQuantidade_DeveMultiplicarPreco()
        {
            var pedido = new Pedido();
            var item = CriarItem("Refrigerante", 2.50m, TipoItem.Bebida);
            pedido.AdicionarItem(item, quantidade: 3);

            pedido.Itens.Should().HaveCount(1);
            pedido.Subtotal.Should().Be(7.50m);
        }

        [Fact]
        public void AdicionarItem_ComObservacao_DeveArmazenar()
        {
            var pedido = new Pedido();
            var item = CriarItem("X Bacon", 7.00m, TipoItem.Sanduiche);
            pedido.AdicionarItem(item, observacao: "Sem cebola");

            pedido.Itens.First().Observacao.Should().Be("Sem cebola");
        }

        [Fact]
        public void AdicionarItem_DeveLancarExcecao_QuandoAdicionarItemDoMesmoTipo()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Burger", 5.00m, TipoItem.Sanduiche));
            Action act = () => pedido.AdicionarItem(CriarItem("X Bacon", 7.00m, TipoItem.Sanduiche));

            act.Should().Throw<DomainException>()
               .WithMessage("O pedido já contém um item do tipo Sanduiche.");
        }

        [Fact]
        public void AdicionarItem_DeveLancarExcecao_QuandoPassarDeTresItens()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Burger", 5.00m, TipoItem.Sanduiche));
            pedido.AdicionarItem(CriarItem("Batata", 2.00m, TipoItem.Acompanhamento));
            pedido.AdicionarItem(CriarItem("Refri", 2.50m, TipoItem.Bebida));

            Action act = () => pedido.AdicionarItem(CriarItem("Extra", 10m, (TipoItem)99));

            act.Should().Throw<DomainException>()
               .WithMessage("O pedido já atingiu o limite máximo de 3 itens.");
        }

        [Fact]
        public void AdicionarItem_QuandoJaTemDesconto_DeveZerarDescontoERecalcular()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Burger", 5.00m, TipoItem.Sanduiche));
            pedido.AplicarPromocoes(new[] { CriarPromocaoTipos("Teste", 0.10m, TipoItem.Sanduiche) });
            pedido.DescontoPercentual.Should().Be(0.10m);

            pedido.AdicionarItem(CriarItem("Batata", 3.00m, TipoItem.Acompanhamento));

            pedido.DescontoPercentual.Should().Be(0m);
            pedido.Subtotal.Should().Be(8.00m);
            pedido.TotalFinal.Should().Be(8.00m);
        }

        #endregion

        #region AdicionarOpcao

        [Fact]
        public void AdicionarOpcao_DeveAumentarSubtotal()
        {
            var pedido = new Pedido();
            var item = CriarItem("X Burger", 5.00m, TipoItem.Sanduiche);
            pedido.AdicionarItem(item);
            var pedidoItem = pedido.Itens.First();

            var grupo = new GrupoOpcao("Adicionais", TipoSelecao.Multipla, false);
            grupo.AdicionarOpcao("Bacon extra", 2.00m);
            var opcao = grupo.Opcoes.First();

            pedidoItem.AdicionarOpcao(opcao, grupo, 2);

            pedidoItem.OpcoesSelecionadas.Should().HaveCount(1);
            pedidoItem.PrecoTotal.Should().Be(9.00m);
            pedido.Subtotal.Should().Be(9.00m);
        }

        #endregion

        #region AplicarPromocoes

        [Fact]
        public void AplicarPromocoes_DeveAplicarOMaiorDesconto_QuandoMultiplasPromocoes()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Burger", 5.00m, TipoItem.Sanduiche));
            pedido.AdicionarItem(CriarItem("Batata", 2.00m, TipoItem.Acompanhamento));

            var p10 = CriarPromocaoTipos("10%", 0.10m, TipoItem.Sanduiche, TipoItem.Acompanhamento);
            var p20 = CriarPromocaoTipos("20%", 0.20m, TipoItem.Sanduiche, TipoItem.Acompanhamento);

            pedido.AplicarPromocoes(new[] { p10, p20 });

            pedido.DescontoPercentual.Should().Be(0.20m);
            pedido.ValorDesconto.Should().Be(1.40m);
            pedido.TotalFinal.Should().Be(5.60m);
        }

        [Fact]
        public void AplicarPromocoes_NaoAplicaSeNenhumaAtende()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Egg", 4.50m, TipoItem.Sanduiche));
            var promo = CriarPromocaoTipos("S+B", 0.10m, TipoItem.Sanduiche, TipoItem.Acompanhamento);

            pedido.AplicarPromocoes(new[] { promo });

            pedido.DescontoPercentual.Should().Be(0m);
            pedido.TotalFinal.Should().Be(4.50m);
        }

        [Fact]
        public void AplicarPromocoes_NaoAplicaSePedidoTemItensAMais()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Burger", 5.00m, TipoItem.Sanduiche));
            pedido.AdicionarItem(CriarItem("Batata", 3.00m, TipoItem.Acompanhamento));
            pedido.AdicionarItem(CriarItem("Refri", 2.00m, TipoItem.Bebida));
            var promo = CriarPromocaoTipos("S+A", 0.10m, TipoItem.Sanduiche, TipoItem.Acompanhamento);

            pedido.AplicarPromocoes(new[] { promo });

            pedido.DescontoPercentual.Should().Be(0m);
            pedido.PromocaoId.Should().BeNull();
        }

        [Fact]
        public void AplicarPromocoes_ItensEspecificos_Funciona()
        {
            var id1 = Guid.NewGuid();
            var pedido = new Pedido();
            var item = CriarItem("X Bacon", 7.00m, TipoItem.Sanduiche, id1);
            pedido.AdicionarItem(item);
            var promo = CriarPromocaoItens("Especial", 0.30m, id1);

            pedido.AplicarPromocoes(new[] { promo });

            pedido.DescontoPercentual.Should().Be(0.30m);
            pedido.ValorDesconto.Should().Be(2.10m);
        }

        [Fact]
        public void AplicarPromocoes_ZeraDescontoComListaVazia()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Burger", 5.00m, TipoItem.Sanduiche));
            pedido.AdicionarItem(CriarItem("Batata", 3.00m, TipoItem.Acompanhamento));
            var promo = CriarPromocaoTipos("Combo", 0.10m, TipoItem.Sanduiche, TipoItem.Acompanhamento);
            pedido.AplicarPromocoes(new[] { promo });
            pedido.DescontoPercentual.Should().Be(0.10m);

            pedido.AplicarPromocoes(Enumerable.Empty<Promocao>());

            pedido.DescontoPercentual.Should().Be(0m);
            pedido.PromocaoId.Should().BeNull();
        }

        #endregion

        #region RemoverItem

        [Fact]
        public void RemoverItem_DeveRemoverERecalcular()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Bacon", 7.00m, TipoItem.Sanduiche));
            pedido.AdicionarItem(CriarItem("Refri", 2.50m, TipoItem.Bebida));
            var sanduicheId = pedido.Itens.First(i => i.Tipo == TipoItem.Sanduiche).Id;

            pedido.RemoverItem(sanduicheId);

            pedido.Itens.Should().HaveCount(1);
            pedido.Itens.First().Tipo.Should().Be(TipoItem.Bebida);
            pedido.Subtotal.Should().Be(2.50m);
        }

        [Fact]
        public void RemoverItem_Inexistente_NaoFazNada()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Bacon", 7.00m, TipoItem.Sanduiche));
            pedido.RemoverItem(Guid.NewGuid());

            pedido.Itens.Should().HaveCount(1);
        }

        [Fact]
        public void RemoverItem_ComDesconto_DeveZerarDesconto()
        {
            var pedido = new Pedido();
            pedido.AdicionarItem(CriarItem("X Burger", 5.00m, TipoItem.Sanduiche));
            pedido.AdicionarItem(CriarItem("Batata", 3.00m, TipoItem.Acompanhamento));
            var promo = CriarPromocaoTipos("Combo", 0.10m, TipoItem.Sanduiche, TipoItem.Acompanhamento);
            pedido.AplicarPromocoes(new[] { promo });
            var batataId = pedido.Itens.First(i => i.Tipo == TipoItem.Acompanhamento).Id;

            pedido.RemoverItem(batataId);

            pedido.DescontoPercentual.Should().Be(0m);
            pedido.TotalFinal.Should().Be(5.00m);
        }

        #endregion

        #region ObservacaoGeral

        [Fact]
        public void DefinirObservacao_DeveArmazenar()
        {
            var pedido = new Pedido();
            pedido.DefinirObservacao("Entregar na mesa 5");
            pedido.ObservacaoGeral.Should().Be("Entregar na mesa 5");
        }

        #endregion
    }
}