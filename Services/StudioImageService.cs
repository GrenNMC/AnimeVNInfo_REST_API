using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IStudioImageService
    {
        public Task<ResponseView> CreateStudioImage(StudioImageView studioImageView, IFormFile file, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken);
        public Task<StudioView> GetStudioById(int id, CancellationToken cancellationToken);
    }
    public class StudioImageService : IStudioImageService
    {
        private readonly IStudioImageRepository _repo;
        private readonly IStudioRepository _studioRepo;

        public StudioImageService(IStudioImageRepository repo, IStudioRepository studioRepo)
        {
            _repo = repo;
            _studioRepo = studioRepo;
        }

        public async Task<ResponseView> CreateStudioImage(StudioImageView studioImageView, IFormFile file, CancellationToken cancellationToken)
        {
            var studio = await _studioRepo.GetStudioById(studioImageView.StudioId ?? 0, cancellationToken);
            if (studio.Id > 0)
            {
                var studioName = (studio.StudioName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
                var folderName = Path.Combine("Image", "Studio", studioName);
                var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
                Directory.CreateDirectory(pathToSave);
                var _res = await _repo.CreateStudioImage(studioImageView, cancellationToken);
                var res = _res.Item1;
                studioImageView.Id = _res.Item2;
                studioImageView.StudioId = _res.Item3;
                if (res.ErrorCode != 0)
                {
                    return res;
                }
                var name = $"{studioName}-{studioImageView.Id}.{file.FileName.Split(".").Last()}";
                var fullPath = Path.Combine(pathToSave, name);
                studioImageView.Image = Path.Combine(folderName, name);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var res2 = await _repo.UpdateStudioImage(studioImageView, cancellationToken);
                if (res2.ErrorCode != 0)
                {
                    return res2;
                }
                return res;
            }
            return new(MessageConstant.NOT_FOUND, 2);
        }

        public async Task<ResponseView> DeleteStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteStudioImage(studioImageView, cancellationToken);
        }

        public async Task<StudioView> GetStudioById(int id, CancellationToken cancellationToken)
        {
            return await _studioRepo.GetStudioById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateStudioImage(studioImageView, cancellationToken);
        }
    }
}
