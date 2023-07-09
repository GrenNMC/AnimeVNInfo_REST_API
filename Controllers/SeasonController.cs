using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class SeasonController : ControllerBase
    {
        private readonly ISeasonService _service;
        private readonly ILoggerService _logService;

        public SeasonController(ISeasonService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateSeason([FromBody] SeasonView seasonView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateSeason(seasonView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    string ss = seasonView.Quarter switch
                    {
                        1 => "Đông",
                        2 => "Xuân",
                        3 => "Hạ",
                        4 => "Thu",
                        _ => ""
                    };
                    await _logService.CreateLog($"{userName.Value} đã tạo mùa mới {ss} {seasonView.Year}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllSeason([FromQuery] SeasonParamView seasonParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllSeason(seasonParamView, cancellationToken);
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteSeason([FromQuery] SeasonView seasonView, CancellationToken cancellationToken)
        {
            var old = await _service.GetSeasonById(seasonView.Id, cancellationToken);
            var res = await _service.DeleteSeason(seasonView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    string ss = old.Quarter switch
                    {
                        1 => "Đông",
                        2 => "Xuân",
                        3 => "Hạ",
                        4 => "Thu",
                        _ => ""
                    };
                    await _logService.CreateLog($"{userName.Value} đã xóa mùa {ss} {old.Year}", cancellationToken);
                }
            }
            return res;
        }
    }
}
