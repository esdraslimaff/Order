using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infra.Mappings
{
    public class GrupoOpcaoConfiguration : IEntityTypeConfiguration<GrupoOpcao>
    {
        public void Configure(EntityTypeBuilder<GrupoOpcao> builder)
        {
            builder.ToTable("GruposOpcoes");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Nome).IsRequired().HasMaxLength(100);
            builder.Property(g => g.TipoSelecao).IsRequired();
            builder.Property(g => g.Obrigatorio).IsRequired();

            builder.HasData(
                new
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Nome = "Adicionais",
                    TipoSelecao = TipoSelecao.Multipla,
                    Obrigatorio = false,
                    MinimoSelecoes = (int?)null,
                    MaximoSelecoes = (int?)3,
                    DataCriacao = new DateTime(2026, 1, 1)
                },
                new
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Nome = "Ponto da carne",
                    TipoSelecao = TipoSelecao.Unica,
                    Obrigatorio = true,
                    MinimoSelecoes = (int?)1,
                    MaximoSelecoes = (int?)1,
                    DataCriacao = new DateTime(2026, 1, 1)
                },
                new
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Nome = "Tamanho",
                    TipoSelecao = TipoSelecao.Unica,
                    Obrigatorio = false,
                    MinimoSelecoes = (int?)null,
                    MaximoSelecoes = (int?)null,
                    DataCriacao = new DateTime(2026, 1, 1)
                }
            );
        }
    }
}