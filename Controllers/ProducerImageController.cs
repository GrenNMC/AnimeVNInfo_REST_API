using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class ProducerImageController : ControllerBase
    {
        private readonly IProducerImageService _service;
        private readonly ILoggerService _logService;

        public ProducerImageController(IProducerImageService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateProducerImage([FromQuery] ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType) return new("Không có file", 999);
            var file = Request.Form.Files[0];
            var res = await _service.CreateProducerImage(producerImageView, file, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var producer = await _service.GetProducerById(producerImageView.ProducerId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã thêm ảnh mới {(producerImageView.Image ?? "").Split("\\").Last()} trong nhà sản xuất {producer.ProducerName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateProducerImage([FromBody] ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            var isAvatar = producerImageView.IsAvatar;
            var res = await _service.UpdateProducerImage(producerImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var producer = await _service.GetProducerById(producerImageView.ProducerId ?? 0, cancellationToken);
                    if (isAvatar)
                    {
                        await _logService.CreateLog($"{userName.Value} đã đặt ảnh {(producerImageView.Image ?? "").Split("\\").Last()} làm ảnh đại diện trong nhà sản xuất {producer.ProducerName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật ảnh {(producerImageView.Image ?? "").Split("\\").Last()} trong nhà sản xuất {producer.ProducerName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteProducerImage([FromQuery] ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            var res = await _service.DeleteProducerImage(producerImageView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var producer = await _service.GetProducerById(producerImageView.ProducerId ?? 0, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã xóa ảnh {(producerImageView.Image ?? "").Split("\\").Last()} trong nhà sản xuất {producer.ProducerName ?? ""}", cancellationToken);
                }
            }
            return res;
        }
    }
}
