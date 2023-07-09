using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IAnimeTypeService
    {
        public Task<ResponseWithPaginationView> GetAllAnimeType(AnimeTypeParamView animeTypeParamView, CancellationToken cancellationToken);
        public Task<AnimeTypeView> GetAnimeTypeById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
    }

    public class AnimeTypeService : IAnimeTypeService
    {
        private readonly IAnimeTypeRepository _repo;

        public AnimeTypeService(IAnimeTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            return await _repo.ChangeNextOrder(animeTypeView, cancellationToken);
        }

        public async Task<ResponseView> ChangePrevOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            return await _repo.ChangePrevOrder(animeTypeView, cancellationToken);
        }

        public async Task<ResponseView> CreateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            return await _repo.CreateAnimeType(animeTypeView, cancellationToken);
        }

        public async Task<ResponseView> DeleteAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAnimeType(animeTypeView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllAnimeType(AnimeTypeParamView animeTypeParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAnimeType(animeTypeParamView, cancellationToken);
        }

        public async Task<AnimeTypeView> GetAnimeTypeById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetAnimeTypeById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateAnimeType(animeTypeView, cancellationToken);
        }
    }
}
