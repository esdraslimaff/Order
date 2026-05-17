using FluentAssertions;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Interfaces.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Application.Tests.Services
{
    public class PromocaoServiceTests
    {
        private readonly Mock<IPromocaoRepository> _promocaoRepoMock;
        private readonly PromocaoService _service;

        public PromocaoServiceTests()
        {
            _promocaoRepoMock = new Mock<IPromocaoRepository>();
            _service = new PromocaoService(_promocaoRepoMock.Object);
        }

        [Fact]
        public async Task ObterAtivasAsync_DeveMapearERetornarPromocoesAtivas()
        {
            var promocao = new Promocao("Combo", 0.10m);
            promocao.AdicionarRequisitoTipo(TipoItem.Sanduiche);
            var listaMock = new List<Promocao> { promocao };

            _promocaoRepoMock.Setup(r => r.ObterTodasAtivasAsync()).ReturnsAsync(listaMock);

            var result = await _service.ObterAtivasAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);

            var dto = result.First();
            dto.Nome.Should().Be("Combo");
            dto.Percentual.Should().Be(0.10m);
            dto.RequisitosTipo.Should().Contain(TipoItem.Sanduiche);
        }
    }
}
