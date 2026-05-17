using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infra.Mappings
{
    public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.ToTable("PedidoItens");
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Nome).IsRequired().HasMaxLength(200);
            builder.Property(pi => pi.PrecoUnitario).HasPrecision(18, 2).IsRequired();
            builder.Property(pi => pi.Quantidade).IsRequired().HasDefaultValue(1);
            builder.Property(pi => pi.Observacao).HasMaxLength(300);

            builder.Property(pi => pi.Id).ValueGeneratedNever();

            builder.HasMany(pi => pi.OpcoesSelecionadas)
                   .WithOne()
                   .HasForeignKey(o => o.PedidoItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}