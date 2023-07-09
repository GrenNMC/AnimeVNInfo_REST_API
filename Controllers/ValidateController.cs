using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class ValidateController : ControllerBase
    {
        private readonly IValidateService _service;

        public ValidateController(IValidateService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ResponseView> GetCode([FromBody] ValidateCodeView validateCodeView, CancellationToken cancellationToken)
        {
            return await _service.GetCode(validateCodeView, cancellationToken);
        }

        [HttpPost]
        public async Task<ResponseView> AdminGetCode([FromBody] ValidateCodeView validateCodeView, CancellationToken cancellationToken)
        {
            return await _service.AdminGetCode(validateCodeView, cancellationToken);
        }
    }
}
