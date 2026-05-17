using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infra.Mappings
{
    public class ItemGrupoOpcaoConfiguration : IEntityTypeConfiguration<ItemGrupoOpcao>
    {
        public void Configure(EntityTypeBuilder<ItemGrupoOpcao> builder)
        {
            builder.ToTable("ItensGruposOpcoes");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Item)
                   .WithMany(i => i.GruposOpcoes)
                   .HasForeignKey(x => x.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GrupoOpcao)
                   .WithMany()
                   .HasForeignKey(x => x.GrupoOpcaoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                // X Burger tem Adicionais (opcional, máx 3) e Ponto da carne (obrigatório)
                new { Id = Guid.Parse("11110000-0001-0000-0000-000000000001"), ItemId = Guid.Parse("A1B2C3D4-E5F6-4A7B-8C9D-0E1F2A3B4C5D"), GrupoOpcaoId = Guid.Parse("11111111-1111-1111-1111-111111111111"), Obrigatorio = false, MinimoSelecoes = (int?)null, MaximoSelecoes = (int?)3, DataCriacao = new DateTime(2026, 1, 1) },
                new { Id = Guid.Parse("11110000-0002-0000-0000-000000000002"), ItemId = Guid.Parse("A1B2C3D4-E5F6-4A7B-8C9D-0E1F2A3B4C5D"), GrupoOpcaoId = Guid.Parse("22222222-2222-2222-2222-222222222222"), Obrigatorio = true, MinimoSelecoes = (int?)1, MaximoSelecoes = (int?)1, DataCriacao = new DateTime(2026, 1, 1) },

                // Batata frita tem Tamanho (opcional)
                new { Id = Guid.Parse("11110000-0003-0000-0000-000000000003"), ItemId = Guid.Parse("D4E5F6A7-B8C9-4D0E-1F2A-3B4C5D6E7F8A"), GrupoOpcaoId = Guid.Parse("33333333-3333-3333-3333-333333333333"), Obrigatorio = false, MinimoSelecoes = (int?)null, MaximoSelecoes = (int?)1, DataCriacao = new DateTime(2026, 1, 1) },

                // Refrigerante tem Tamanho
                new { Id = Guid.Parse("11110000-0004-0000-0000-000000000004"), ItemId = Guid.Parse("E5F6A7B8-C9D0-4E1F-2A3B-4C5D6E7F8A9B"), GrupoOpcaoId = Guid.Parse("33333333-3333-3333-3333-333333333333"), Obrigatorio = false, MinimoSelecoes = (int?)null, MaximoSelecoes = (int?)1, DataCriacao = new DateTime(2026, 1, 1) }
            );
        }
    }
}