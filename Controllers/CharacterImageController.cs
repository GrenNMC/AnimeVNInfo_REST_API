using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class CharacterImageController : ControllerBase
    {
        private readonly ICharacterImageService _service;
        private readonly ILoggerService _logService;

        public CharacterImageController(ICharacterImageService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateCharacterImage([FromQuery] CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType) return new("Không có file", 999);
            var file = Request.Form.Files[0];
            var res = await _service.CreateCharacterImage(characterImageView, file, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var character = await _service.GetCharacterById(characterImageView.CharacterId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã thêm ảnh mới {(characterImageView.Image ?? "").Split("\\").Last()} cho nhân vật {character.NativeName ?? ""} {character.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateCharacterImage([FromBody] CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            var isAvatar = characterImageView.IsAvatar;
            var res = await _service.UpdateCharacterImage(characterImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var character = await _service.GetCharacterById(characterImageView.CharacterId ?? 0, cancellationToken);
                    if (isAvatar)
                    {
                        await _logService.CreateLog($"{userName.Value} đã đặt ảnh {(characterImageView.Image ?? "").Split("\\").Last()} làm ảnh đại diện cho nhân vật {character.NativeName ?? ""} {character.LatinName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật ảnh {(characterImageView.Image ?? "").Split("\\").Last()} cho nhân vật {character.NativeName ?? ""} {character.LatinName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteCharacterImage([FromQuery] CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            var res = await _service.DeleteCharacterImage(characterImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var character = await _service.GetCharacterById(characterImageView.CharacterId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã xóa ảnh {(characterImageView.Image ?? "").Split("\\").Last()} cho nhân vật {character.NativeName ?? ""} {character.LatinName ?? ""}", cancellationToken);
                }
            }
            return res;
        }
    }
}
