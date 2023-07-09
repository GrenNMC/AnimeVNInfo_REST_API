using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IRatingService
    {
        public Task<ResponseWithPaginationView> GetAllRating(RatingParamView ratingParamView, CancellationToken cancellationToken);
        public Task<RatingView> GetRatingById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateRating(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateRating(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteRating(RatingView ratingView, CancellationToken cancellationToken);
    }

    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _repo;

        public RatingService(IRatingRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> ChangeNextOrder(RatingView ratingView, CancellationToken cancellationToken)
        {
            return await _repo.ChangeNextOrder(ratingView, cancellationToken);
        }

        public async Task<ResponseView> ChangePrevOrder(RatingView ratingView, CancellationToken cancellationToken)
        {
            return await _repo.ChangePrevOrder(ratingView, cancellationToken);
        }

        public async Task<ResponseView> CreateRating(RatingView ratingView, CancellationToken cancellationToken)
        {
            return await _repo.CreateRating(ratingView, cancellationToken);
        }

        public async Task<ResponseView> DeleteRating(RatingView ratingView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteRating(ratingView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllRating(RatingParamView ratingParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllRating(ratingParamView, cancellationToken);
        }

        public async Task<RatingView> GetRatingById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetRatingById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateRating(RatingView ratingView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateRating(ratingView, cancellationToken);
        }
    }
}
