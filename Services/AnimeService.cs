using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IAnimeService
    {
        public Task<ResponseWithPaginationView> GetAllAnime(AnimeParamView animeParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAll(AnimeParamView animeParamView, CancellationToken cancellationToken);
        public Task<AnimeView> GetAnimeById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnime(AnimeView animeView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnime(AnimeView animeView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnime(AnimeView animeView, CancellationToken cancellationToken);
    }

    public class AnimeService : IAnimeService
    {
        private readonly IAnimeRepository _repo;
        private readonly IAnimeImageRepository _siRepo;

        public AnimeService(IAnimeRepository repo, IAnimeImageRepository siRepo)
        {
            _repo = repo;
            _siRepo = siRepo;
        }

        public async Task<ResponseView> CreateAnime(AnimeView animeView, CancellationToken cancellationToken)
        {
            return await _repo.CreateAnime(animeView, cancellationToken);
        }

        public async Task<ResponseView> DeleteAnime(AnimeView animeView, CancellationToken cancellationToken)
        {
            var res = await _siRepo.DeleteAllAnimeImage(animeView.Id, cancellationToken);
            if (res.ErrorCode != 0)
            {
                return res;
            }
            res = await _repo.DeleteAnime(animeView, cancellationToken);
            return res;
        }

        public async Task<ResponseWithPaginationView> GetAllAnime(AnimeParamView animeParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAnime(animeParamView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAll(AnimeParamView animeParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAll(animeParamView, cancellationToken);
        }

        public async Task<AnimeView> GetAnimeById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetAnimeById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateAnime(AnimeView animeView, CancellationToken cancellationToken)
        {
            var newName = (animeView.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var id = animeView.Id;
            var _res = await _repo.UpdateAnime(animeView, cancellationToken);
            var res = _res.Item1;
            animeView.LatinName = _res.Item2;
            animeView.NativeName = _res.Item3;
            if (res.ErrorCode != 0)
            {
                return res;
            }
            var oldName = (animeView.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var folderName = Path.Combine("Image", "Anime");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, $"{oldName}-{id}");
            if (Directory.Exists(pathToSave) && newName != oldName)
            {
                var newPath = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, $"{newName}-{id}");
                Directory.Move(pathToSave, newPath);
                var files = Directory.GetFiles(newPath);
                foreach (var file in files)
                {
                    var name = $"{newName}-{id}-{file.Split("-").Last()}";
                    File.Move(file, $"{file.Substring(0, file.LastIndexOf("\\"))}\\{name}", true);
                }
                res = await _siRepo.UpdateAllNameAnimeImage(id, newName, cancellationToken);
            }
            return res;
        }
    }
}
