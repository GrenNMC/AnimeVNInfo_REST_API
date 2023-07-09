using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AnimeListStatusController : ControllerBase
    {
        private readonly IAnimeListStatusService _service;
        private readonly ILoggerService _logService;

        public AnimeListStatusController(IAnimeListStatusService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateAnimeListStatus([FromBody] AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateAnimeListStatus(animeListStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo trạng thái list anime mới {animeListStatusView.StatusName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangeNextOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeListStatusById(animeListStatusView.Id, cancellationToken);
            var res = await _service.ChangeNextOrder(animeListStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeListStatusById(animeListStatusView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa trạng thái list anime {news.StatusName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangePrevOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeListStatusById(animeListStatusView.Id, cancellationToken);
            var res = await _service.ChangePrevOrder(animeListStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeListStatusById(animeListStatusView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa trạng thái list anime {news.StatusName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetAllAnimeListStatus([FromQuery] AnimeListStatusParamView animeListStatusParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllAnimeListStatus(animeListStatusParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteAnimeListStatus([FromQuery] AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeListStatusById(animeListStatusView.Id, cancellationToken);
            var res = await _service.DeleteAnimeListStatus(animeListStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa trạng thái list anime {old.StatusName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateAnimeListStatus([FromBody] AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeListStatusById(animeListStatusView.Id, cancellationToken);
            var res = await _service.UpdateAnimeListStatus(animeListStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.StatusName == animeListStatusView.StatusName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật trạng thái list anime {old.StatusName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật trạng thái list anime {old.StatusName ?? ""} thành {animeListStatusView.StatusName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
