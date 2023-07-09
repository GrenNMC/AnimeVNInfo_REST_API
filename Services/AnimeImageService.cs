using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IAnimeImageService
    {
        public Task<ResponseView> CreateAnimeImage(AnimeImageView animeImageView, IFormFile file, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken);
        public Task<AnimeView> GetAnimeById(int id, CancellationToken cancellationToken);
    }
    public class AnimeImageService : IAnimeImageService
    {
        private readonly IAnimeImageRepository _repo;
        private readonly IAnimeRepository _animeRepo;

        public AnimeImageService(IAnimeImageRepository repo, IAnimeRepository animeRepo)
        {
            _repo = repo;
            _animeRepo = animeRepo;
        }

        public async Task<ResponseView> CreateAnimeImage(AnimeImageView animeImageView, IFormFile file, CancellationToken cancellationToken)
        {
            var anime = await _animeRepo.GetAnimeById(animeImageView.AnimeId ?? 0, cancellationToken);
            if (anime.Id > 0)
            {
                var romanjiName = (anime.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
                var folderName = Path.Combine("Image", "Anime", $"{romanjiName}-{anime.Id}");
                var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
                Directory.CreateDirectory(pathToSave);
                var _res = await _repo.CreateAnimeImage(animeImageView, cancellationToken);
                var res = _res.Item1;
                animeImageView.Id = _res.Item2;
                animeImageView.AnimeId = _res.Item3;
                if (res.ErrorCode != 0)
                {
                    return res;
                }
                var name = $"{romanjiName}-{anime.Id}-{animeImageView.Id}.{file.FileName.Split(".").Last()}";
                var fullPath = Path.Combine(pathToSave, name);
                animeImageView.Image = Path.Combine(folderName, name);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var res2 = await _repo.UpdateAnimeImage(animeImageView, cancellationToken);
                if (res2.ErrorCode != 0)
                {
                    return res2;
                }
                return res;
            }
            return new(MessageConstant.NOT_FOUND, 2);
        }

        public async Task<ResponseView> DeleteAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAnimeImage(animeImageView, cancellationToken);
        }

        public async Task<AnimeView> GetAnimeById(int id, CancellationToken cancellationToken)
        {
            return await _animeRepo.GetAnimeById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateAnimeImage(animeImageView, cancellationToken);
        }
    }
}
