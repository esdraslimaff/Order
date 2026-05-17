using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Infra.Repositories
{
    public class ItemRepository : RepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Item>> GetItensPorIdsAsync(IEnumerable<Guid> ids)
        {
            return await DbSet
                .Where(i => ids.Contains(i.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetAllWithGruposAsync()
        {
            return await Context.Itens
                .Include(i => i.GruposOpcoes)
                    .ThenInclude(ig => ig.GrupoOpcao)
                        .ThenInclude(g => g.Opcoes)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Item?> GetByIdWithGruposOpcoesAsync(Guid id)
        {
            return await Context.Itens
                .Include(i => i.GruposOpcoes)
                    .ThenInclude(ig => ig.GrupoOpcao)
                        .ThenInclude(g => g.Opcoes)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
