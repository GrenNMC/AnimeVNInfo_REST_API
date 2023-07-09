using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    [AdminAuthorize]
    public class LoggerController : ControllerBase
    {
        private readonly ILoggerService _service;

        public LoggerController(ILoggerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ResponseWithPaginationView> GetAllLog([FromQuery] LoggerParamView loggerParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllLog(loggerParamView, cancellationToken);
        }
    }
}
