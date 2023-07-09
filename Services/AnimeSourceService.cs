using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IAnimeSourceService
    {
        public Task<ResponseWithPaginationView> GetAllAnimeSource(AnimeSourceParamView animeSourceParamView, CancellationToken cancellationToken);
        public Task<AnimeSourceView> GetAnimeSourceById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
    }

    public class AnimeSourceService : IAnimeSourceService
    {
        private readonly IAnimeSourceRepository _repo;

        public AnimeSourceService(IAnimeSourceRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            return await _repo.ChangeNextOrder(animeSourceView, cancellationToken);
        }

        public async Task<ResponseView> ChangePrevOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            return await _repo.ChangePrevOrder(animeSourceView, cancellationToken);
        }

        public async Task<ResponseView> CreateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            return await _repo.CreateAnimeSource(animeSourceView, cancellationToken);
        }

        public async Task<ResponseView> DeleteAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAnimeSource(animeSourceView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllAnimeSource(AnimeSourceParamView animeSourceParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAnimeSource(animeSourceParamView, cancellationToken);
        }

        public async Task<AnimeSourceView> GetAnimeSourceById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetAnimeSourceById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateAnimeSource(animeSourceView, cancellationToken);
        }
    }
}
