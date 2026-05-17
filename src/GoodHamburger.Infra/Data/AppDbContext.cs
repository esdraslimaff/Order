using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<Item> Itens => Set<Item>();
        public DbSet<Promocao> Promocao => Set<Promocao>();
        public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<GrupoOpcao> GruposOpcoes { get; set; }
        public DbSet<Opcao> Opcoes { get; set; }
        public DbSet<ItemGrupoOpcao> ItensGruposOpcoes { get; set; }
        public DbSet<PedidoItemOpcao> PedidoItensOpcoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
