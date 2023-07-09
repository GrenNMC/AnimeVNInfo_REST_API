using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface ISeasonService
    {
        public Task<ResponseWithPaginationView> GetAllSeason(SeasonParamView seasonParamView, CancellationToken cancellationToken);
        public Task<SeasonView> GetSeasonById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateSeason(SeasonView seasonView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteSeason(SeasonView seasonView, CancellationToken cancellationToken);
    }

    public class SeasonService : ISeasonService
    {
        private readonly ISeasonRepository _repo;

        public SeasonService(ISeasonRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> CreateSeason(SeasonView seasonView, CancellationToken cancellationToken)
        {
            return await _repo.CreateSeason(seasonView, cancellationToken);
        }

        public async Task<ResponseView> DeleteSeason(SeasonView seasonView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteSeason(seasonView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllSeason(SeasonParamView seasonParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllSeason(seasonParamView, cancellationToken);
        }

        public async Task<SeasonView> GetSeasonById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetSeasonById(id, cancellationToken);
        }
    }
}
