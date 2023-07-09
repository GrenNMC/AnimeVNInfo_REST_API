using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _service;
        private readonly ILoggerService _logService;

        public LanguageController(ILanguageService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateLanguage([FromBody] LanguageView LanguageView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateLanguage(LanguageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo ngôn ngữ mới {LanguageView.LanguageName}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllLanguage([FromQuery] LanguageParamView LanguageParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllLanguage(LanguageParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteLanguage([FromQuery] LanguageView LanguageView, CancellationToken cancellationToken)
        {
            var old = await _service.GetLanguageById(LanguageView.Id, cancellationToken);
            var res = await _service.DeleteLanguage(LanguageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa ngôn ngữ {old.LanguageName}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateLanguage([FromBody] LanguageView LanguageView, CancellationToken cancellationToken)
        {
            var old = await _service.GetLanguageById(LanguageView.Id, cancellationToken);
            var res = await _service.UpdateLanguage(LanguageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.LanguageName == LanguageView.LanguageName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật ngôn ngữ {old.LanguageName}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật ngôn ngữ {old.LanguageName} thành {LanguageView.LanguageName}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
