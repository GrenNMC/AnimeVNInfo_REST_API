using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class StudioImageController : ControllerBase
    {
        private readonly IStudioImageService _service;
        private readonly ILoggerService _logService;

        public StudioImageController(IStudioImageService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateStudioImage([FromQuery] StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType) return new("Không có file", 999);
            var file = Request.Form.Files[0];
            var res = await _service.CreateStudioImage(studioImageView, file, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var studio = await _service.GetStudioById(studioImageView.StudioId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã thêm ảnh mới {(studioImageView.Image ?? "").Split("\\").Last()} trong studio {studio.StudioName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateStudioImage([FromBody] StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            var isAvatar = studioImageView.IsAvatar;
            var res = await _service.UpdateStudioImage(studioImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var studio = await _service.GetStudioById(studioImageView.StudioId ?? 0, cancellationToken);
                    if (isAvatar)
                    {
                        await _logService.CreateLog($"{userName.Value} đã đặt ảnh {(studioImageView.Image ?? "").Split("\\").Last()} làm ảnh đại diện trong studio {studio.StudioName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật ảnh {(studioImageView.Image ?? "").Split("\\").Last()} trong studio {studio.StudioName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteStudioImage([FromQuery] StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            var res = await _service.DeleteStudioImage(studioImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var studio = await _service.GetStudioById(studioImageView.StudioId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã xóa ảnh {(studioImageView.Image ?? "").Split("\\").Last()} trong studio {studio.StudioName ?? ""}", cancellationToken);
                }
            }
            return res;
        }
    }
}
