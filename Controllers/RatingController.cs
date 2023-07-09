using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _service;
        private readonly ILoggerService _logService;

        public RatingController(IRatingService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateRating([FromBody] RatingView ratingView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateRating(ratingView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo xếp loại mới {ratingView.RatingName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangeNextOrder(RatingView ratingView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRatingById(ratingView.Id, cancellationToken);
            var res = await _service.ChangeNextOrder(ratingView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetRatingById(ratingView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa xếp loại {news.RatingName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangePrevOrder(RatingView ratingView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRatingById(ratingView.Id, cancellationToken);
            var res = await _service.ChangePrevOrder(ratingView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var news = await _service.GetRatingById(ratingView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa xếp loại {news.RatingName} từ thứ tự {old.Order} về thứ tự {news.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllRating([FromQuery] RatingParamView ratingParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllRating(ratingParamView, cancellationToken);
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteRating([FromQuery] RatingView ratingView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRatingById(ratingView.Id, cancellationToken);
            var res = await _service.DeleteRating(ratingView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa xếp loại {old.RatingName ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateRating([FromBody] RatingView ratingView, CancellationToken cancellationToken)
        {
            var old = await _service.GetRatingById(ratingView.Id, cancellationToken);
            var res = await _service.UpdateRating(ratingView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (old.RatingName == ratingView.RatingName)
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật xếp loại {old.RatingName ?? ""}", cancellationToken);
                    }
                    else
                    {
                        await _logService.CreateLog($"{userName.Value} đã cập nhật xếp loại {old.RatingName ?? ""} thành {ratingView.RatingName ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
