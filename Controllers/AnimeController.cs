using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using AnimeVnInfoBackend.Utilities.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeService _service;
        private readonly ILoggerService _logService;

        public AnimeController(IAnimeService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateAnime([FromBody] AnimeView animeView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateAnime(animeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo anime mới {animeView.NativeName ?? ""} {animeView.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteAnime([FromQuery] AnimeView animeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeById(animeView.Id, cancellationToken);
            var res = await _service.DeleteAnime(animeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa anime {old.NativeName ?? ""} {old.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllAnime([FromQuery] AnimeParamView animeParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllAnime(animeParamView, cancellationToken);
        }

        [HttpGet]
        public async Task<ResponseWithPaginationView> GetAll([FromQuery] AnimeParamView animeParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAll(animeParamView, cancellationToken);
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateAnime([FromBody] AnimeView animeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeById(animeView.Id, cancellationToken);
            var newName = animeView.LatinName;
            var newJName = animeView.NativeName;
            var res = await _service.UpdateAnime(animeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.LatinName == newName && old.NativeName == newJName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật anime {old.NativeName ?? ""} {old.LatinName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật anime {old.NativeName ?? ""} {old.LatinName ?? ""} thành {newJName ?? ""} {newName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
