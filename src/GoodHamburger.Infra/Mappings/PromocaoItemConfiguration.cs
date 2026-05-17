//using GoodHamburger.Domain.Entities;
//using GoodHamburger.Domain.Enums;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GoodHamburger.Infra.Mappings
//{
//    public class PromocaoItemConfiguration : IEntityTypeConfiguration<PromocaoItem>
//    {
//        public void Configure(EntityTypeBuilder<PromocaoItem> builder)
//        {
//            builder.ToTable("PromocaoItens");

//            builder.HasKey(x => x.Id);

//            builder.Property(x => x.TipoItem)
//                .HasConversion<int>()
//                .IsRequired();

//            builder.Property<Guid>("PromocaoId");

//            builder.HasData(
//                            new
//                            {
//                                Id = Guid.Parse("a7d3c1f0-2e4b-4d9a-8c1f-5e7b2a6c9001"),
//                                PromocaoId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d471"),
//                                TipoItem = TipoItem.Sanduiche,
//                                DataCriacao = DateTime.UtcNow
//                            },
//                            new
//                            {
//                                Id = Guid.Parse("b8e4d2a1-3c5f-4a8b-9d2e-6f1a3c7b9002"),
//                                PromocaoId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d471"),
//                                TipoItem = TipoItem.Bebida,
//                                DataCriacao = DateTime.UtcNow
//                            },
//                            new
//                            {
//                                Id = Guid.Parse("c9f5e3b2-4d6a-4b7c-8e3f-7a2b4d8c9003"),
//                                PromocaoId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d471"),
//                                TipoItem = TipoItem.Acompanhamento,
//                                DataCriacao = DateTime.UtcNow
//                            },

//                            new
//                            {
//                                Id = Guid.Parse("e1b2c3d4-6f7a-4b8c-9d0e-8a1b2c3d4004"),
//                                PromocaoId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d472"),
//                                TipoItem = TipoItem.Sanduiche,
//                                DataCriacao = DateTime.UtcNow
//                            },
//                            new
//                            {
//                                Id = Guid.Parse("f2c3d4e5-7a8b-4c9d-0e1f-9b2c3d4e5005"),
//                                PromocaoId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d472"),
//                                TipoItem = TipoItem.Bebida,
//                                DataCriacao = DateTime.UtcNow
//                            },

//                            new
//                            {
//                                Id = Guid.Parse("a4d5e6f7-9b0c-4d1e-8f2a-2b3c4d5e6006"),
//                                PromocaoId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d473"),
//                                TipoItem = TipoItem.Sanduiche,
//                                DataCriacao = DateTime.UtcNow
//                            },
//                            new
//                            {
//                                Id = Guid.Parse("b5e6f7a8-0c1d-4e2f-9a3b-3c4d5e6f7007"),
//                                PromocaoId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d473"),
//                                TipoItem = TipoItem.Acompanhamento,
//                                DataCriacao = DateTime.UtcNow
//                            }
//                        );
//        }
//    }
//}
