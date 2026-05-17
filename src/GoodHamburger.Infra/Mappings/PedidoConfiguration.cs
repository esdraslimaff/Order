using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infra.Mappings
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.DataCriacao).IsRequired();
            builder.Property(p => p.DescontoPercentual).HasPrecision(5, 2);
            builder.Property(p => p.Subtotal).HasPrecision(18, 2);
            builder.Property(p => p.ValorDesconto).HasPrecision(18, 2);
            builder.Property(p => p.TotalFinal).HasPrecision(18, 2);
            builder.Property(p => p.ObservacaoGeral).HasMaxLength(500);

            builder.HasMany(p => p.Itens)
                   .WithOne()
                   .HasForeignKey("PedidoId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}