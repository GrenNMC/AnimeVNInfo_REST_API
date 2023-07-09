using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class RelationTypeController : ControllerBase
    {
        private readonly IRelationTypeService _service;
        private readonly ILoggerService _logService;

        public RelationTypeController(IRelationTypeService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateRelationType([FromBody] RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateRelationType(relationTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo loại liên kết mới {relationTypeView.TypeName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangeNextOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRelationTypeById(relationTypeView.Id, cancellationToken);
            var res = await _service.ChangeNextOrder(relationTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetRelationTypeById(relationTypeView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa loại liên kết {news.TypeName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangePrevOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRelationTypeById(relationTypeView.Id, cancellationToken);
            var res = await _service.ChangePrevOrder(relationTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetRelationTypeById(relationTypeView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa loại liên kết {news.TypeName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllRelationType([FromQuery] RelationTypeParamView relationTypeParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllRelationType(relationTypeParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteRelationType([FromQuery] RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRelationTypeById(relationTypeView.Id, cancellationToken);
            var res = await _service.DeleteRelationType(relationTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa loại liên kết {old.TypeName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateRelationType([FromBody] RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRelationTypeById(relationTypeView.Id, cancellationToken);
            var res = await _service.UpdateRelationType(relationTypeView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.TypeName == relationTypeView.TypeName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật loại liên kết {old.TypeName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật loại liên kết {old.TypeName ?? ""} thành {relationTypeView.TypeName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
