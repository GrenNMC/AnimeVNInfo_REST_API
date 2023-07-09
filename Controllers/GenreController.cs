using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _service;
        private readonly ILoggerService _logService;

        public GenreController(IGenreService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateGenre([FromBody] GenreView genreView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateGenre(genreView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo thể loại mới {genreView.GenreName}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllGenre([FromQuery] GenreParamView genreParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllGenre(genreParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteGenre([FromQuery] GenreView genreView, CancellationToken cancellationToken)
        {
            var old = await _service.GetGenreById(genreView.Id, cancellationToken);
            var res = await _service.DeleteGenre(genreView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa thể loại {old.GenreName}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateGenre([FromBody] GenreView genreView, CancellationToken cancellationToken)
        {
            var old = await _service.GetGenreById(genreView.Id, cancellationToken);
            var res = await _service.UpdateGenre(genreView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.GenreName == genreView.GenreName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật thể loại {old.GenreName}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật thể loại {old.GenreName} thành {genreView.GenreName}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
