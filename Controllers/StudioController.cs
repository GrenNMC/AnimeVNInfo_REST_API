using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class StudioController : ControllerBase
    {
        private readonly IStudioService _service;
        private readonly ILoggerService _logService;

        public StudioController(IStudioService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateStudio([FromBody] StudioView studioView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateStudio(studioView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo studio mới {studioView.StudioName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteStudio([FromQuery] StudioView studioView, CancellationToken cancellationToken)
        {
            var old = await _service.GetStudioById(studioView.Id, cancellationToken);
            var res = await _service.DeleteStudio(studioView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa studio {old.StudioName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllStudio([FromQuery] StudioParamView studioParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllStudio(studioParamView, cancellationToken);
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetStudioByIdAnilist([FromBody] StudioQueryListView studioQueryListView, CancellationToken cancellationToken)
        {
            return await _service.GetAllStudio(studioQueryListView, cancellationToken);
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateStudio([FromBody] StudioView studioView, CancellationToken cancellationToken)
        {
            var old = await _service.GetStudioById(studioView.Id, cancellationToken);
            var newName = studioView.StudioName;
            var res = await _service.UpdateStudio(studioView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.StudioName == newName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật studio {old.StudioName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật studio {old.StudioName ?? ""} thành {newName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
