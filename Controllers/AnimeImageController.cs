using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AnimeImageController : ControllerBase
    {
        private readonly IAnimeImageService _service;
        private readonly ILoggerService _logService;

        public AnimeImageController(IAnimeImageService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateAnimeImage([FromQuery] AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType) return new("Không có file", 999);
            var file = Request.Form.Files[0];
            var res = await _service.CreateAnimeImage(animeImageView, file, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var anime = await _service.GetAnimeById(animeImageView.AnimeId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã thêm ảnh mới {(animeImageView.Image ?? "").Split("\\").Last()} cho nhân vật {anime.NativeName ?? ""} {anime.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateAnimeImage([FromBody] AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            var isAvatar = animeImageView.IsAvatar;
            var res = await _service.UpdateAnimeImage(animeImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var anime = await _service.GetAnimeById(animeImageView.AnimeId ?? 0, cancellationToken);
                    if (isAvatar)
                    {
                        await _logService.CreateLog($"{userName.Value} đã đặt ảnh {(animeImageView.Image ?? "").Split("\\").Last()} làm ảnh đại diện cho nhân vật {anime.NativeName ?? ""} {anime.LatinName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật ảnh {(animeImageView.Image ?? "").Split("\\").Last()} cho nhân vật {anime.NativeName ?? ""} {anime.LatinName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteAnimeImage([FromQuery] AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            var res = await _service.DeleteAnimeImage(animeImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var anime = await _service.GetAnimeById(animeImageView.AnimeId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã xóa ảnh {(animeImageView.Image ?? "").Split("\\").Last()} cho nhân vật {anime.NativeName ?? ""} {anime.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }
    }
}
