using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AnimeTypeController : ControllerBase
    {
        private readonly IAnimeTypeService _service;
        private readonly ILoggerService _logService;

        public AnimeTypeController(IAnimeTypeService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateAnimeType([FromBody] AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateAnimeType(animeTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo loại hình anime mới {animeTypeView.TypeName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangeNextOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeTypeById(animeTypeView.Id, cancellationToken);
            var res = await _service.ChangeNextOrder(animeTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeTypeById(animeTypeView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa loại hình anime {news.TypeName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangePrevOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeTypeById(animeTypeView.Id, cancellationToken);
            var res = await _service.ChangePrevOrder(animeTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetAnimeTypeById(animeTypeView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa loại hình anime {news.TypeName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllAnimeType([FromQuery] AnimeTypeParamView animeTypeParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllAnimeType(animeTypeParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteAnimeType([FromQuery] AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeTypeById(animeTypeView.Id, cancellationToken);
            var res = await _service.DeleteAnimeType(animeTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa loại hình anime {old.TypeName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateAnimeType([FromBody] AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetAnimeTypeById(animeTypeView.Id, cancellationToken);
            var res = await _service.UpdateAnimeType(animeTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.TypeName == animeTypeView.TypeName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật loại hình anime {old.TypeName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật loại hình anime {old.TypeName ?? ""} thành {animeTypeView.TypeName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
