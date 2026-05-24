using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoodHamburger.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InicializacaoBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GruposOpcoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoSelecao = table.Column<int>(type: "int", nullable: false),
                    Obrigatorio = table.Column<bool>(type: "bit", nullable: false),
                    MinimoSelecoes = table.Column<int>(type: "int", nullable: true),
                    MaximoSelecoes = table.Column<int>(type: "int", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposOpcoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Itens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DescontoPercentual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ValorDesconto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFinal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PromocaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ObservacaoGeral = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promocao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Percentual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    RequisitosTipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItensObrigatoriosIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promocao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Perfil = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Opcoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrecoAdicional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrupoOpcaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opcoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opcoes_GruposOpcoes_GrupoOpcaoId",
                        column: x => x.GrupoOpcaoId,
                        principalTable: "GruposOpcoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensGruposOpcoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrupoOpcaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Obrigatorio = table.Column<bool>(type: "bit", nullable: false),
                    MinimoSelecoes = table.Column<int>(type: "int", nullable: true),
                    MaximoSelecoes = table.Column<int>(type: "int", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensGruposOpcoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensGruposOpcoes_GruposOpcoes_GrupoOpcaoId",
                        column: x => x.GrupoOpcaoId,
                        principalTable: "GruposOpcoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensGruposOpcoes_Itens_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Itens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Observacao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItensOpcoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpcaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeOpcao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NomeGrupo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItensOpcoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItensOpcoes_PedidoItens_PedidoItemId",
                        column: x => x.PedidoItemId,
                        principalTable: "PedidoItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GruposOpcoes",
                columns: new[] { "Id", "DataCriacao", "DataUltimaAlteracao", "MaximoSelecoes", "MinimoSelecoes", "Nome", "Obrigatorio", "TipoSelecao" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, null, "Adicionais", false, 2 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 1, "Ponto da carne", true, 1 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Tamanho", false, 1 }
                });

            migrationBuilder.InsertData(
                table: "Itens",
                columns: new[] { "Id", "DataCriacao", "DataUltimaAlteracao", "Nome", "PrecoUnitario", "Tipo" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), new DateTime(2026, 5, 24, 17, 46, 57, 147, DateTimeKind.Utc).AddTicks(9835), null, "X Burger", 5.00m, 1 },
                    { new Guid("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), new DateTime(2026, 5, 24, 17, 46, 57, 147, DateTimeKind.Utc).AddTicks(9839), null, "X Egg", 4.50m, 1 },
                    { new Guid("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), new DateTime(2026, 5, 24, 17, 46, 57, 147, DateTimeKind.Utc).AddTicks(9840), null, "X Bacon", 7.00m, 1 },
                    { new Guid("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), new DateTime(2026, 5, 24, 17, 46, 57, 147, DateTimeKind.Utc).AddTicks(9841), null, "Batata frita", 2.00m, 2 },
                    { new Guid("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), new DateTime(2026, 5, 24, 17, 46, 57, 147, DateTimeKind.Utc).AddTicks(9842), null, "Refrigerante", 2.50m, 3 }
                });

            migrationBuilder.InsertData(
                table: "Promocao",
                columns: new[] { "Id", "Ativo", "DataCriacao", "DataUltimaAlteracao", "ItensObrigatoriosIds", "Nome", "Percentual", "RequisitosTipo" },
                values: new object[,]
                {
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d471"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "Combo Completo", 0.20m, "1,3,2" },
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d472"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "Lanche e Refri", 0.15m, "1,3" },
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d473"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "Lanche e Batata", 0.10m, "1,2" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "DataCriacao", "DataUltimaAlteracao", "Email", "Nome", "Perfil", "SenhaHash" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@goodhamburger.com", "Administrador", 1, "$2a$12$MxtoBxZcMUBXeCfpog03a.SbV2eeH/kbRsBGmK8oY.1rIMtgF6Uti" });

            migrationBuilder.InsertData(
                table: "ItensGruposOpcoes",
                columns: new[] { "Id", "DataCriacao", "DataUltimaAlteracao", "GrupoOpcaoId", "ItemId", "MaximoSelecoes", "MinimoSelecoes", "Obrigatorio" },
                values: new object[,]
                {
                    { new Guid("11110000-0001-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), 3, null, false },
                    { new Guid("11110000-0002-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("22222222-2222-2222-2222-222222222222"), new Guid("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), 1, 1, true },
                    { new Guid("11110000-0003-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("33333333-3333-3333-3333-333333333333"), new Guid("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), 1, null, false },
                    { new Guid("11110000-0004-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("33333333-3333-3333-3333-333333333333"), new Guid("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), 1, null, false }
                });

            migrationBuilder.InsertData(
                table: "Opcoes",
                columns: new[] { "Id", "DataCriacao", "DataUltimaAlteracao", "GrupoOpcaoId", "Nome", "PrecoAdicional" },
                values: new object[,]
                {
                    { new Guid("a0000000-0001-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("11111111-1111-1111-1111-111111111111"), "Bacon extra", 2.00m },
                    { new Guid("a0000000-0002-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("11111111-1111-1111-1111-111111111111"), "Queijo extra", 1.50m },
                    { new Guid("a0000000-0003-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("11111111-1111-1111-1111-111111111111"), "Molho barbecue", 1.00m },
                    { new Guid("b0000000-0001-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("22222222-2222-2222-2222-222222222222"), "Mal passada", 0.00m },
                    { new Guid("b0000000-0002-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("22222222-2222-2222-2222-222222222222"), "Ao ponto", 0.00m },
                    { new Guid("b0000000-0003-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("22222222-2222-2222-2222-222222222222"), "Bem passada", 0.00m },
                    { new Guid("c0000000-0001-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("33333333-3333-3333-3333-333333333333"), "Pequeno", -2.00m },
                    { new Guid("c0000000-0002-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("33333333-3333-3333-3333-333333333333"), "Médio", 0.00m },
                    { new Guid("c0000000-0003-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("33333333-3333-3333-3333-333333333333"), "Grande", 3.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensGruposOpcoes_GrupoOpcaoId",
                table: "ItensGruposOpcoes",
                column: "GrupoOpcaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensGruposOpcoes_ItemId",
                table: "ItensGruposOpcoes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Opcoes_GrupoOpcaoId",
                table: "Opcoes",
                column: "GrupoOpcaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_PedidoId",
                table: "PedidoItens",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItensOpcoes_PedidoItemId",
                table: "PedidoItensOpcoes",
                column: "PedidoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensGruposOpcoes");

            migrationBuilder.DropTable(
                name: "Opcoes");

            migrationBuilder.DropTable(
                name: "PedidoItensOpcoes");

            migrationBuilder.DropTable(
                name: "Promocao");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Itens");

            migrationBuilder.DropTable(
                name: "GruposOpcoes");

            migrationBuilder.DropTable(
                name: "PedidoItens");

            migrationBuilder.DropTable(
                name: "Pedidos");
        }
    }
}
