using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IReviewScoreService
    {
        public Task<ResponseWithPaginationView> GetAllReviewScore(ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllDeletedReviewScore(ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken);
        public Task<ReviewScoreView> GetReviewScoreById(int id, CancellationToken cancellationToken);
        public Task<List<ReviewScoreView>> GetReviewScoreByListId(List<int> ids, CancellationToken cancellationToken);
        public Task<ResponseView> CreateReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteReviewScorePermanently(List<int> ids, CancellationToken cancellationToken);
    }

    public class ReviewScoreService : IReviewScoreService
    {
        private readonly IReviewScoreRepository _repo;

        public ReviewScoreService(IReviewScoreRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> CreateReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            return await _repo.CreateReviewScore(reviewScoreView, cancellationToken);
        }

        public async Task<ResponseView> DeleteReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteReviewScore(reviewScoreView, cancellationToken);
        }

        public async Task<ResponseView> DeleteReviewScorePermanently(List<int> ids, CancellationToken cancellationToken) {
            return await _repo.DeleteReviewScorePermanently(ids, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllDeletedReviewScore(ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken) {
            return await _repo.GetAllDeletedReviewScore(reviewScoreParamView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllReviewScore(ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllReviewScore(reviewScoreParamView, cancellationToken);
        }

        public async Task<ReviewScoreView> GetReviewScoreById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetReviewScoreById(id, cancellationToken);
        }

        public async Task<List<ReviewScoreView>> GetReviewScoreByListId(List<int> ids, CancellationToken cancellationToken) {
            return await _repo.GetReviewScoreByListId(ids, cancellationToken);
        }

        public async Task<ResponseView> UpdateReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateReviewScore(reviewScoreView, cancellationToken);
        }
    }
}
