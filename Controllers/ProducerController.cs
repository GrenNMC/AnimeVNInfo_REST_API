using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly IProducerService _service;
        private readonly ILoggerService _logService;

        public ProducerController(IProducerService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [ModeratorAuthorize]
        public async Task<ResponseView> CreateProducer([FromBody] ProducerView producerView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateProducer(producerView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo nhà sản xuất mới {producerView.ProducerName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpDelete]
        [ModeratorAuthorize]
        public async Task<ResponseView> DeleteProducer([FromQuery] ProducerView producerView, CancellationToken cancellationToken)
        {
            var old = await _service.GetProducerById(producerView.Id, cancellationToken);
            var res = await _service.DeleteProducer(producerView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa nhà sản xuất {old.ProducerName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllProducer([FromQuery] ProducerParamView producerParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllProducer(producerParamView, cancellationToken);
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetProducerByIdAnilist([FromBody] ProducerQueryListView producerQueryListView, CancellationToken cancellationToken)
        {
            return await _service.GetAllProducer(producerQueryListView, cancellationToken);
        }

        [HttpPut]
        [ModeratorAuthorize]
        public async Task<ResponseView> UpdateProducer([FromBody] ProducerView producerView, CancellationToken cancellationToken)
        {
            var old = await _service.GetProducerById(producerView.Id, cancellationToken);
            var newName = producerView.ProducerName;
            var res = await _service.UpdateProducer(producerView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.ProducerName == newName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nhà sản xuất {old.ProducerName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nhà sản xuất {old.ProducerName ?? ""} thành {newName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
