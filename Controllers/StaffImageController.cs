using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class StaffImageController : ControllerBase
    {
        private readonly IStaffImageService _service;
        private readonly ILoggerService _logService;

        public StaffImageController(IStaffImageService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateStaffImage([FromQuery] StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType) return new("Không có file", 999);
            var file = Request.Form.Files[0];
            var res = await _service.CreateStaffImage(staffImageView, file, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var staff = await _service.GetStaffById(staffImageView.StaffId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã thêm ảnh mới {(staffImageView.Image ?? "").Split("\\").Last()} cho nhân viên {staff.NativeName ?? ""} {staff.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateStaffImage([FromBody] StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            var isAvatar = staffImageView.IsAvatar;
            var res = await _service.UpdateStaffImage(staffImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var staff = await _service.GetStaffById(staffImageView.StaffId ?? 0, cancellationToken);
                    if (isAvatar)
                    {
                        await _logService.CreateLog($"{userName.Value} đã đặt ảnh {(staffImageView.Image ?? "").Split("\\").Last()} làm ảnh đại diện cho nhân viên {staff.NativeName ?? ""} {staff.LatinName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật ảnh {(staffImageView.Image ?? "").Split("\\").Last()} cho nhân viên {staff.NativeName ?? ""} {staff.LatinName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteStaffImage([FromQuery] StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            var res = await _service.DeleteStaffImage(staffImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var staff = await _service.GetStaffById(staffImageView.StaffId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã xóa ảnh {(staffImageView.Image ?? "").Split("\\").Last()} cho nhân viên {staff.NativeName ?? ""} {staff.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }
    }
}
