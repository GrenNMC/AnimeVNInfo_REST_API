using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IGenreService
    {
        public Task<ResponseWithPaginationView> GetAllGenre(GenreParamView genreParamView, CancellationToken cancellationToken);
        public Task<GenreView> GetGenreById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateGenre(GenreView genreView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateGenre(GenreView genreView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteGenre(GenreView genreView, CancellationToken cancellationToken);
    }

    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _repo;

        public GenreService(IGenreRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> CreateGenre(GenreView genreView, CancellationToken cancellationToken)
        {
            return await _repo.CreateGenre(genreView, cancellationToken);
        }

        public async Task<ResponseView> DeleteGenre(GenreView genreView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteGenre(genreView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllGenre(GenreParamView genreParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllGenre(genreParamView, cancellationToken);
        }

        public async Task<GenreView> GetGenreById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetGenreById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateGenre(GenreView genreView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateGenre(genreView, cancellationToken);
        }
    }
}
