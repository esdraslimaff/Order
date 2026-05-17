using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Interfaces.Repository;
using GoodHamburger.Shared.DTOs;

namespace GoodHamburger.Application.Services
{
    public class PromocaoService : IPromocaoService
    {
        private readonly IPromocaoRepository _repository;

        public PromocaoService(IPromocaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PromocaoDto>> ObterAtivasAsync()
        {
            var promocoes = await _repository.ObterTodasAtivasAsync();
            return promocoes.Select(MapToDto);
        }

        public async Task<IEnumerable<PromocaoDto>> ObterTodasPromocoesAsync()
        {
            var promocoes = await _repository.ObterTodasPromocoesComRequisitosAsync();
            return promocoes.Select(MapToDto);
        }

        public async Task<PromocaoDto> BuscarPromocaoComRequisitosPorIdAsync(Guid id)
        {
            var promocao = await _repository.BuscarPromocaoComRequisitosPorIdAsync(id);
            return promocao == null ? null : MapToDto(promocao);
        }

        public async Task AlternarStatusAsync(Guid id)
        {
            var promocao = await _repository.GetByIdAsync(id)
                ?? throw new DomainException("Promoção não encontrada.");

            promocao.AlternarStatus();
            await _repository.UpdateAsync(promocao);
        }

        private static PromocaoDto MapToDto(Promocao promocao)
        {
            return new PromocaoDto
            {
                Id = promocao.Id,
                Nome = promocao.Nome,
                Percentual = promocao.Percentual,
                Ativo = promocao.Ativo,
                RequisitosTipo = promocao.RequisitosTipo?.ToList() ?? new List<TipoItem>(),
                ItensObrigatoriosIds = promocao.ItensObrigatoriosIds?.ToList() ?? new List<Guid>()
            };
        }
    }
}