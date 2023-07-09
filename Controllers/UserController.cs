using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using AnimeVnInfoBackend.Utilities.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IValidateService _validateService;

        public UserController(IUserService service, IValidateService validateService)
        {
            _service = service;
            _validateService = validateService;
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetAllUser([FromQuery] UserParamView userParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllUser(userParamView, cancellationToken);
        }

        [HttpGet]
        public async Task<UserRolesAndDisableStatusView> GetRolesAndDisableStatus([FromQuery] UserView userView, CancellationToken cancellationToken)
        {
            return await _service.GetRolesAndDisableStatus(userView, cancellationToken);
        }

        [HttpPost]
        [UserAuthorize]
        public async Task<ResponseView> UpdateUser([FromBody] UserView userView, CancellationToken cancellationToken)
        {
            return await _service.UpdateUser(userView, cancellationToken);
        }
    }
}
