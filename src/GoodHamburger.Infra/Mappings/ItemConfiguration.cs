using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infra.Mappings
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Itens");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.PrecoUnitario).HasPrecision(18, 2).IsRequired();
            builder.Property(i => i.Tipo).IsRequired();

            builder.HasMany(i => i.GruposOpcoes)
                   .WithOne(ig => ig.Item)
                   .HasForeignKey(ig => ig.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new { Id = Guid.Parse("A1B2C3D4-E5F6-4A7B-8C9D-0E1F2A3B4C5D"), Nome = "X Burger", PrecoUnitario = 5.00m, Tipo = TipoItem.Sanduiche, DataCriacao = DateTime.UtcNow },
                new { Id = Guid.Parse("B2C3D4E5-F6A7-4B8C-9D0E-1F2A3B4C5D6E"), Nome = "X Egg", PrecoUnitario = 4.50m, Tipo = TipoItem.Sanduiche, DataCriacao = DateTime.UtcNow },
                new { Id = Guid.Parse("C3D4E5F6-A7B8-4C9D-0E1F-2A3B4C5D6E7F"), Nome = "X Bacon", PrecoUnitario = 7.00m, Tipo = TipoItem.Sanduiche, DataCriacao = DateTime.UtcNow },
                new { Id = Guid.Parse("D4E5F6A7-B8C9-4D0E-1F2A-3B4C5D6E7F8A"), Nome = "Batata frita", PrecoUnitario = 2.00m, Tipo = TipoItem.Acompanhamento, DataCriacao = DateTime.UtcNow },
                new { Id = Guid.Parse("E5F6A7B8-C9D0-4E1F-2A3B-4C5D6E7F8A9B"), Nome = "Refrigerante", PrecoUnitario = 2.50m, Tipo = TipoItem.Bebida, DataCriacao = DateTime.UtcNow }
            );
        }
    }
}