using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infra.Mappings
{
    public class OpcaoConfiguration : IEntityTypeConfiguration<Opcao>
    {
        public void Configure(EntityTypeBuilder<Opcao> builder)
        {
            builder.ToTable("Opcoes");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Nome).IsRequired().HasMaxLength(100);
            builder.Property(o => o.PrecoAdicional).HasPrecision(18, 2).IsRequired();

            builder.HasOne(o => o.GrupoOpcao)
                   .WithMany(g => g.Opcoes)
                   .HasForeignKey(o => o.GrupoOpcaoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                // Adicionais (grupo 1111...)
                new { Id = Guid.Parse("A0000000-0001-0000-0000-000000000001"), Nome = "Bacon extra", PrecoAdicional = 2.00m, GrupoOpcaoId = Guid.Parse("11111111-1111-1111-1111-111111111111"), DataCriacao = new DateTime(2026, 1, 1) },
                new { Id = Guid.Parse("A0000000-0002-0000-0000-000000000002"), Nome = "Queijo extra", PrecoAdicional = 1.50m, GrupoOpcaoId = Guid.Parse("11111111-1111-1111-1111-111111111111"), DataCriacao = new DateTime(2026, 1, 1) },
                new { Id = Guid.Parse("A0000000-0003-0000-0000-000000000003"), Nome = "Molho barbecue", PrecoAdicional = 1.00m, GrupoOpcaoId = Guid.Parse("11111111-1111-1111-1111-111111111111"), DataCriacao = new DateTime(2026, 1, 1) },

                // Ponto da carne (grupo 2222...)
                new { Id = Guid.Parse("B0000000-0001-0000-0000-000000000001"), Nome = "Mal passada", PrecoAdicional = 0.00m, GrupoOpcaoId = Guid.Parse("22222222-2222-2222-2222-222222222222"), DataCriacao = new DateTime(2026, 1, 1) },
                new { Id = Guid.Parse("B0000000-0002-0000-0000-000000000002"), Nome = "Ao ponto", PrecoAdicional = 0.00m, GrupoOpcaoId = Guid.Parse("22222222-2222-2222-2222-222222222222"), DataCriacao = new DateTime(2026, 1, 1) },
                new { Id = Guid.Parse("B0000000-0003-0000-0000-000000000003"), Nome = "Bem passada", PrecoAdicional = 0.00m, GrupoOpcaoId = Guid.Parse("22222222-2222-2222-2222-222222222222"), DataCriacao = new DateTime(2026, 1, 1) },

                // Tamanho (grupo 3333...)
                new { Id = Guid.Parse("C0000000-0001-0000-0000-000000000001"), Nome = "Pequeno", PrecoAdicional = -2.00m, GrupoOpcaoId = Guid.Parse("33333333-3333-3333-3333-333333333333"), DataCriacao = new DateTime(2026, 1, 1) },
                new { Id = Guid.Parse("C0000000-0002-0000-0000-000000000002"), Nome = "Médio", PrecoAdicional = 0.00m, GrupoOpcaoId = Guid.Parse("33333333-3333-3333-3333-333333333333"), DataCriacao = new DateTime(2026, 1, 1) },
                new { Id = Guid.Parse("C0000000-0003-0000-0000-000000000003"), Nome = "Grande", PrecoAdicional = 3.00m, GrupoOpcaoId = Guid.Parse("33333333-3333-3333-3333-333333333333"), DataCriacao = new DateTime(2026, 1, 1) }
            );
        }
    }
}