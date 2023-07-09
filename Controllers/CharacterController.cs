using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _service;
        private readonly ILoggerService _logService;

        public CharacterController(ICharacterService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateCharacter([FromBody] CharacterView characterView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateCharacter(characterView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo nhân vật mới {characterView.NativeName ?? ""} {characterView.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteCharacter([FromQuery] CharacterView characterView, CancellationToken cancellationToken)
        {
            var old = await _service.GetCharacterById(characterView.Id, cancellationToken);
            var res = await _service.DeleteCharacter(characterView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa nhân vật {old.NativeName ?? ""} {old.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllCharacter([FromQuery] CharacterParamView characterParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllCharacter(characterParamView, cancellationToken);
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetCharacterByIdAnilist([FromBody] CharacterQueryListView characterQueryListView, CancellationToken cancellationToken)
        {
            return await _service.GetAllCharacter(characterQueryListView, cancellationToken);
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateCharacter([FromBody] CharacterView characterView, CancellationToken cancellationToken)
        {
            var old = await _service.GetCharacterById(characterView.Id, cancellationToken);
            var newName = characterView.LatinName;
            var newJName = characterView.NativeName;
            var res = await _service.UpdateCharacter(characterView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.LatinName == newName && old.NativeName == newJName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nhân vật {old.NativeName ?? ""} {old.LatinName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nhân vật {old.NativeName ?? ""} {old.LatinName ?? ""} thành {newJName ?? ""} {newName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
