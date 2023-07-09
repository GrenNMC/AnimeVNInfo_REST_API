using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AnimeSourceController : ControllerBase
    {
        private readonly IAnimeSourceService _service;
        private readonly ILoggerService _logService;

        public AnimeSourceController(IAnimeSourceService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateAnimeSource([FromBody] AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateAnimeSource(animeSourceView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo nguồn gốc anime mới {animeSourceView.SourceName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangeNextOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeSourceById(animeSourceView.Id, cancellationToken);
            var res = await _service.ChangeNextOrder(animeSourceView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeSourceById(animeSourceView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa nguồn gốc anime {news.SourceName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangePrevOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeSourceById(animeSourceView.Id, cancellationToken);
            var res = await _service.ChangePrevOrder(animeSourceView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeSourceById(animeSourceView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa nguồn gốc anime {news.SourceName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllAnimeSource([FromQuery] AnimeSourceParamView animeSourceParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllAnimeSource(animeSourceParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteAnimeSource([FromQuery] AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeSourceById(animeSourceView.Id, cancellationToken);
            var res = await _service.DeleteAnimeSource(animeSourceView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa nguồn gốc anime {old.SourceName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateAnimeSource([FromBody] AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeSourceById(animeSourceView.Id, cancellationToken);
            var res = await _service.UpdateAnimeSource(animeSourceView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.SourceName == animeSourceView.SourceName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nguồn gốc anime {old.SourceName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nguồn gốc anime {old.SourceName ?? ""} thành {animeSourceView.SourceName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
