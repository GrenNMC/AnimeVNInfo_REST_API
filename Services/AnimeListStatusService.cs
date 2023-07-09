using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IAnimeListStatusService
    {
        public Task<ResponseWithPaginationView> GetAllAnimeListStatus(AnimeListStatusParamView animeListStatusParamView, CancellationToken cancellationToken);
        public Task<AnimeListStatusView> GetAnimeListStatusById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
    }

    public class AnimeListStatusService : IAnimeListStatusService
    {
        private readonly IAnimeListStatusRepository _repo;

        public AnimeListStatusService(IAnimeListStatusRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            return await _repo.ChangeNextOrder(animeListStatusView, cancellationToken);
        }

        public async Task<ResponseView> ChangePrevOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            return await _repo.ChangePrevOrder(animeListStatusView, cancellationToken);
        }

        public async Task<ResponseView> CreateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            return await _repo.CreateAnimeListStatus(animeListStatusView, cancellationToken);
        }

        public async Task<ResponseView> DeleteAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAnimeListStatus(animeListStatusView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllAnimeListStatus(AnimeListStatusParamView animeListStatusParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAnimeListStatus(animeListStatusParamView, cancellationToken);
        }

        public async Task<AnimeListStatusView> GetAnimeListStatusById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetAnimeListStatusById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateAnimeListStatus(animeListStatusView, cancellationToken);
        }
    }
}
