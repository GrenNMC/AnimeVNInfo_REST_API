using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IStudioService
    {
        public Task<ResponseWithPaginationView> GetAllStudio(StudioParamView studioParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllStudio(StudioQueryListView studioQueryListView, CancellationToken cancellationToken);
        public Task<StudioView> GetStudioById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateStudio(StudioView studioView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateStudio(StudioView studioView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStudio(StudioView studioView, CancellationToken cancellationToken);
    }

    public class StudioService : IStudioService
    {
        private readonly IStudioRepository _repo;
        private readonly IStudioImageRepository _siRepo;

        public StudioService(IStudioRepository repo, IStudioImageRepository siRepo)
        {
            _repo = repo;
            _siRepo = siRepo;
        }

        public async Task<ResponseView> CreateStudio(StudioView studioView, CancellationToken cancellationToken)
        {
            return await _repo.CreateStudio(studioView, cancellationToken);
        }

        public async Task<ResponseView> DeleteStudio(StudioView studioView, CancellationToken cancellationToken)
        {
            var res = await _siRepo.DeleteAllStudioImage(studioView.Id, cancellationToken);
            if (res.ErrorCode != 0)
            {
                return res;
            }
            res = await _repo.DeleteStudio(studioView, cancellationToken);
            return res;
        }

        public async Task<ResponseWithPaginationView> GetAllStudio(StudioParamView studioParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllStudio(studioParamView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllStudio(StudioQueryListView studioQueryListView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllStudio(studioQueryListView, cancellationToken);
        }

        public async Task<StudioView> GetStudioById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetStudioById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateStudio(StudioView studioView, CancellationToken cancellationToken)
        {
            var newName = (studioView.StudioName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var _res = await _repo.UpdateStudio(studioView, cancellationToken);
            var res = _res.Item1;
            studioView.StudioName = _res.Item2;
            if (res.ErrorCode != 0)
            {
                return res;
            }
            var oldName = (studioView.StudioName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var folderName = Path.Combine("Image", "Studio");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, oldName);
            if (Directory.Exists(pathToSave) && newName != oldName)
            {
                var newPath = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, newName);
                Directory.Move(pathToSave, newPath);
                var files = Directory.GetFiles(newPath);
                foreach (var file in files)
                {
                    var name = $"{newName}-{file.Split("-").Last()}";
                    File.Move(file, $"{file.Substring(0, file.LastIndexOf("\\"))}\\{name}", true);
                }
                res = await _siRepo.UpdateAllNameStudioImage(studioView.Id, newName, cancellationToken);
            }
            return res;
        }
    }
}
