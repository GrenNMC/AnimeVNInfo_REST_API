using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AnimeStatusController : ControllerBase
    {
        private readonly IAnimeStatusService _service;
        private readonly ILoggerService _logService;

        public AnimeStatusController(IAnimeStatusService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateAnimeStatus([FromBody] AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateAnimeStatus(animeStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo trạng thái anime mới {animeStatusView.StatusName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangeNextOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeStatusById(animeStatusView.Id, cancellationToken);
            var res = await _service.ChangeNextOrder(animeStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeStatusById(animeStatusView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa trạng thái anime {news.StatusName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangePrevOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeStatusById(animeStatusView.Id, cancellationToken);
            var res = await _service.ChangePrevOrder(animeStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeStatusById(animeStatusView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa trạng thái anime {news.StatusName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllAnimeStatus([FromQuery] AnimeStatusParamView animeStatusParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllAnimeStatus(animeStatusParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteAnimeStatus([FromQuery] AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeStatusById(animeStatusView.Id, cancellationToken);
            var res = await _service.DeleteAnimeStatus(animeStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa trạng thái anime {old.StatusName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateAnimeStatus([FromBody] AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeStatusById(animeStatusView.Id, cancellationToken);
            var res = await _service.UpdateAnimeStatus(animeStatusView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.StatusName == animeStatusView.StatusName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật trạng thái anime {old.StatusName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật trạng thái anime {old.StatusName ?? ""} thành {animeStatusView.StatusName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
