using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IStaffImageService
    {
        public Task<ResponseView> CreateStaffImage(StaffImageView staffImageView, IFormFile file, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken);
        public Task<StaffView> GetStaffById(int id, CancellationToken cancellationToken);
    }
    public class StaffImageService : IStaffImageService
    {
        private readonly IStaffImageRepository _repo;
        private readonly IStaffRepository _staffRepo;

        public StaffImageService(IStaffImageRepository repo, IStaffRepository staffRepo)
        {
            _repo = repo;
            _staffRepo = staffRepo;
        }

        public async Task<ResponseView> CreateStaffImage(StaffImageView staffImageView, IFormFile file, CancellationToken cancellationToken)
        {
            var staff = await _staffRepo.GetStaffById(staffImageView.StaffId ?? 0, cancellationToken);
            if (staff.Id > 0)
            {
                var romanjiName = (staff.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
                var folderName = Path.Combine("Image", "Staff", $"{romanjiName}-{staff.Id}");
                var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
                Directory.CreateDirectory(pathToSave);
                var _res = await _repo.CreateStaffImage(staffImageView, cancellationToken);
                var res = _res.Item1;
                staffImageView.Id = _res.Item2;
                staffImageView.StaffId = _res.Item3;
                if (res.ErrorCode != 0)
                {
                    return res;
                }
                var name = $"{romanjiName}-{staff.Id}-{staffImageView.Id}.{file.FileName.Split(".").Last()}";
                var fullPath = Path.Combine(pathToSave, name);
                staffImageView.Image = Path.Combine(folderName, name);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var res2 = await _repo.UpdateStaffImage(staffImageView, cancellationToken);
                if (res2.ErrorCode != 0)
                {
                    return res2;
                }
                return res;
            }
            return new(MessageConstant.NOT_FOUND, 2);
        }

        public async Task<ResponseView> DeleteStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteStaffImage(staffImageView, cancellationToken);
        }

        public async Task<StaffView> GetStaffById(int id, CancellationToken cancellationToken)
        {
            return await _staffRepo.GetStaffById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateStaffImage(staffImageView, cancellationToken);
        }
    }
}
