using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class ReviewScoreController : ControllerBase
    {
        private readonly IReviewScoreService _service;
        private readonly ILoggerService _logService;

        public ReviewScoreController(IReviewScoreService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateReviewScore([FromBody] ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateReviewScore(reviewScoreView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo điểm đánh giá mới {reviewScoreView.Score}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetAllReviewScore([FromQuery] ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllReviewScore(reviewScoreParamView, cancellationToken);
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<ResponseWithPaginationView> GetAllDeletedReviewScore([FromQuery] ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken) {
            return await _service.GetAllDeletedReviewScore(reviewScoreParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteReviewScore([FromQuery] ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            var old = await _service.GetReviewScoreById(reviewScoreView.Id, cancellationToken);
            var res = await _service.DeleteReviewScore(reviewScoreView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa điểm đánh giá {old.Score}", cancellationToken);
                }
            }
            return res;
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteReviewScorePermanently([FromQuery] List<int> id, CancellationToken cancellationToken) {
            var olds = await _service.GetReviewScoreByListId(id, cancellationToken);
            var res = await _service.DeleteReviewScorePermanently(id, cancellationToken);
            if (res.ErrorCode == 0) {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null) {
                    var logs = new List<string>();
                    foreach(var old in olds) {
                        logs.Add($"{userName.Value} đã xóa vĩnh viễn điểm đánh giá {old.Score}");
                    }
                    await _logService.CreateMultipleLog(logs, cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateReviewScore([FromBody] ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            var old = await _service.GetReviewScoreById(reviewScoreView.Id, cancellationToken);
            var res = await _service.UpdateReviewScore(reviewScoreView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.Score == reviewScoreView.Score)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật điểm đánh giá {old.Score}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật điểm đánh giá {old.Score} thành {reviewScoreView.Score}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
