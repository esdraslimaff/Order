using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace GoodHamburger.Infra.Mappings
{
    public class PromocaoConfiguration : IEntityTypeConfiguration<Promocao>
    {
        public void Configure(EntityTypeBuilder<Promocao> builder)
        {
            builder.ToTable("Promocao");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Percentual)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(r => r.Ativo)
                .IsRequired();

            builder.Property(r => r.DataCriacao)
                .IsRequired();

            builder.Property(r => r.RequisitosTipo)
                .HasConversion(
                    new ValueConverter<List<TipoItem>, string>(
                        v => string.Join(",", v.Select(t => (int)t)),
                        v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                              .Select(int.Parse).Cast<TipoItem>().ToList()
                    ),
                    new ValueComparer<List<TipoItem>>(
                        (c1, c2) => c1!.SequenceEqual(c2!),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );

            builder.Property(r => r.ItensObrigatoriosIds)
                .HasConversion(
                    new ValueConverter<List<Guid>, string>(
                        v => string.Join(",", v),
                        v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                              .Select(Guid.Parse).ToList()
                    ),
                    new ValueComparer<List<Guid>>(
                        (c1, c2) => c1!.SequenceEqual(c2!),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );

            builder.HasData(
                new
                {
                    Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d471"),
                    Nome = "Combo Completo",
                    Percentual = 0.20m,
                    Ativo = true,
                    DataCriacao = new DateTime(2026, 1, 1),
                    RequisitosTipo = new List<TipoItem> { TipoItem.Sanduiche, TipoItem.Bebida, TipoItem.Acompanhamento },
                    ItensObrigatoriosIds = new List<Guid>()
                },
                new
                {
                    Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d472"),
                    Nome = "Lanche e Refri",
                    Percentual = 0.15m,
                    Ativo = true,
                    DataCriacao = new DateTime(2026, 1, 1),
                    RequisitosTipo = new List<TipoItem> { TipoItem.Sanduiche, TipoItem.Bebida },
                    ItensObrigatoriosIds = new List<Guid>()
                },
                new
                {
                    Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d473"),
                    Nome = "Lanche e Batata",
                    Percentual = 0.10m,
                    Ativo = true,
                    DataCriacao = new DateTime(2026, 1, 1),
                    RequisitosTipo = new List<TipoItem> { TipoItem.Sanduiche, TipoItem.Acompanhamento },
                    ItensObrigatoriosIds = new List<Guid>()
                }
            );
        }
    }
}