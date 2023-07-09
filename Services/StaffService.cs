using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IStaffService
    {
        public Task<ResponseWithPaginationView> GetAllStaff(StaffParamView staffParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllStaff(StaffQueryListView staffQueryListView, CancellationToken cancellationToken);
        public Task<StaffView> GetStaffById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateStaff(StaffView staffView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateStaff(StaffView staffView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStaff(StaffView staffView, CancellationToken cancellationToken);
    }

    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repo;
        private readonly IStaffImageRepository _siRepo;

        public StaffService(IStaffRepository repo, IStaffImageRepository siRepo)
        {
            _repo = repo;
            _siRepo = siRepo;
        }

        public async Task<ResponseView> CreateStaff(StaffView staffView, CancellationToken cancellationToken)
        {
            return await _repo.CreateStaff(staffView, cancellationToken);
        }

        public async Task<ResponseView> DeleteStaff(StaffView staffView, CancellationToken cancellationToken)
        {
            var res = await _siRepo.DeleteAllStaffImage(staffView.Id, cancellationToken);
            if (res.ErrorCode != 0)
            {
                return res;
            }
            res = await _repo.DeleteStaff(staffView, cancellationToken);
            return res;
        }

        public async Task<ResponseWithPaginationView> GetAllStaff(StaffParamView staffParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllStaff(staffParamView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllStaff(StaffQueryListView staffQueryListView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllStaff(staffQueryListView, cancellationToken);
        }

        public async Task<StaffView> GetStaffById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetStaffById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateStaff(StaffView staffView, CancellationToken cancellationToken)
        {
            var newName = (staffView.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var id = staffView.Id;
            var _res = await _repo.UpdateStaff(staffView, cancellationToken);
            var res = _res.Item1;
            staffView.LatinName = _res.Item2;
            staffView.NativeName = _res.Item3;
            if (res.ErrorCode != 0)
            {
                return res;
            }
            var oldName = (staffView.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var folderName = Path.Combine("Image", "Staff");
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
                res = await _siRepo.UpdateAllNameStaffImage(id, newName, cancellationToken);
            }
            return res;
        }
    }
}
