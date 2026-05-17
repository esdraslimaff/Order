using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Domain.Interfaces
{
    public interface IItemRepository : IBaseRepository<Item>
    {
        Task<IEnumerable<Item>> GetItensPorIdsAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<Item>> GetAllWithGruposAsync();
        Task<Item?> GetByIdWithGruposOpcoesAsync(Guid id);
    }
}
