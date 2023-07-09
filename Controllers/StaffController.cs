using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;
        private readonly ILoggerService _logService;

        public StaffController(IStaffService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateStaff([FromBody] StaffView staffView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateStaff(staffView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo nhân viên mới {staffView.NativeName ?? ""} {staffView.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteStaff([FromQuery] StaffView staffView, CancellationToken cancellationToken)
        {
            var old = await _service.GetStaffById(staffView.Id, cancellationToken);
            var res = await _service.DeleteStaff(staffView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa nhân viên {old.NativeName ?? ""} {old.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllStaff([FromQuery] StaffParamView staffParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllStaff(staffParamView, cancellationToken);
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetStaffByIdAnilist([FromBody] StaffQueryListView staffQueryListView, CancellationToken cancellationToken)
        {
            return await _service.GetAllStaff(staffQueryListView, cancellationToken);
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateStaff([FromBody] StaffView staffView, CancellationToken cancellationToken)
        {
            var old = await _service.GetStaffById(staffView.Id, cancellationToken);
            var newName = staffView.LatinName;
            var newJName = staffView.NativeName;
            var res = await _service.UpdateStaff(staffView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.LatinName == newName && old.NativeName == newJName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nhân viên {old.NativeName ?? ""} {old.LatinName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nhân viên {old.NativeName ?? ""} {old.LatinName ?? ""} thành {newJName ?? ""} {newName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
