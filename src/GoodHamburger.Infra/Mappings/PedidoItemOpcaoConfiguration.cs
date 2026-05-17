using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infra.Mappings
{
    public class PedidoItemOpcaoConfiguration : IEntityTypeConfiguration<PedidoItemOpcao>
    {
        public void Configure(EntityTypeBuilder<PedidoItemOpcao> builder)
        {
            builder.ToTable("PedidoItensOpcoes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.NomeOpcao).IsRequired().HasMaxLength(200);
            builder.Property(x => x.NomeGrupo).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PrecoUnitario).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Quantidade).IsRequired().HasDefaultValue(1);

            builder.Property(o => o.Id).ValueGeneratedNever();

            builder.HasOne<PedidoItem>()
                   .WithMany(pi => pi.OpcoesSelecionadas)
                   .HasForeignKey(x => x.PedidoItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}