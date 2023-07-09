using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _service;
        private readonly IValidateService _validateService;

        public LogController(ILogService service, IValidateService validateService)
        {
            _service = service;
            _validateService = validateService;
        }

        [HttpPost]
        public async Task<ResponseLoginView> AdminLogin([FromBody] LoginView loginView, CancellationToken cancellationToken)
        {
            var log = await _service.AdminLogin(loginView, cancellationToken);
            if (log.ErrorCode == 0)
            {
                if (string.IsNullOrWhiteSpace(loginView.Code))
                {
                    return new(0, null, MessageConstant.CODE_NOT_ENTER, 11, null);
                }
                ValidateCodeView validateCodeView = new()
                {
                    Code = loginView.Code,
                    UserName = loginView.UserName
                };
                var validate = await _validateService.ValidateCode(validateCodeView, cancellationToken);
                if (validate.ErrorCode != 0)
                {
                    return new(0, null, validate.Message, validate.ErrorCode, null);
                }
            }
            return log;
        }

        [HttpPost]
        public async Task<ResponseLoginView> Login([FromBody] LoginView loginView, CancellationToken cancellationToken)
        {
            return await _service.Login(loginView, cancellationToken);
        }
    }
}
