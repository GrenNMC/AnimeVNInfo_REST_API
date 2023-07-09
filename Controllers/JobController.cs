using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _service;
        private readonly ILoggerService _logService;

        public JobController(IJobService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateJob([FromBody] JobView JobView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateJob(JobView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo nghề nghiệp mới {JobView.JobName}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllJob([FromQuery] JobParamView JobParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllJob(JobParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteJob([FromQuery] JobView JobView, CancellationToken cancellationToken)
        {
            var old = await _service.GetJobById(JobView.Id, cancellationToken);
            var res = await _service.DeleteJob(JobView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa nghề nghiệp {old.JobName}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateJob([FromBody] JobView JobView, CancellationToken cancellationToken)
        {
            var old = await _service.GetJobById(JobView.Id, cancellationToken);
            var res = await _service.UpdateJob(JobView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.JobName == JobView.JobName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nghề nghiệp {old.JobName}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật nghề nghiệp {old.JobName} thành {JobView.JobName}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
