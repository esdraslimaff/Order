using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Shared.DTOs
{
    public class ItemCardapioDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal PrecoUnitario { get; set; }
        public TipoItem Tipo { get; set; }
        public List<GrupoOpcaoDto> GruposOpcoes { get; set; } = new();
    }

    public class GrupoOpcaoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public TipoSelecao TipoSelecao { get; set; }
        public bool Obrigatorio { get; set; }
        public int? MinimoSelecoes { get; set; }
        public int? MaximoSelecoes { get; set; }
        public List<OpcaoDto> Opcoes { get; set; } = new();
    }

    public class OpcaoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal PrecoAdicional { get; set; }
    }
}