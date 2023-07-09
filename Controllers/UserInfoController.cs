using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IUserInfoService _service;

        public UserInfoController(IUserInfoService service)
        {
            _service = service;
        }

        [HttpPut]
        public async Task<ResponseView> UpdateUserInfo([FromBody] UserInfoView userInfoView, CancellationToken cancellationToken)
        {
            return await _service.UpdateUserInfo(userInfoView, cancellationToken);
        }
    }
}
