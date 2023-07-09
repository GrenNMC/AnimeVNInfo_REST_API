using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IAnimeStatusService
    {
        public Task<ResponseWithPaginationView> GetAllAnimeStatus(AnimeStatusParamView animeStatusParamView, CancellationToken cancellationToken);
        public Task<AnimeStatusView> GetAnimeStatusById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
    }

    public class AnimeStatusService : IAnimeStatusService
    {
        private readonly IAnimeStatusRepository _repo;

        public AnimeStatusService(IAnimeStatusRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            return await _repo.ChangeNextOrder(animeStatusView, cancellationToken);
        }

        public async Task<ResponseView> ChangePrevOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            return await _repo.ChangePrevOrder(animeStatusView, cancellationToken);
        }

        public async Task<ResponseView> CreateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            return await _repo.CreateAnimeStatus(animeStatusView, cancellationToken);
        }

        public async Task<ResponseView> DeleteAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAnimeStatus(animeStatusView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllAnimeStatus(AnimeStatusParamView animeStatusParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAnimeStatus(animeStatusParamView, cancellationToken);
        }

        public async Task<AnimeStatusView> GetAnimeStatusById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetAnimeStatusById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateAnimeStatus(animeStatusView, cancellationToken);
        }
    }
}
