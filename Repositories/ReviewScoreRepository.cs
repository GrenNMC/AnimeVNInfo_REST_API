using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IReviewScoreRepository
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

    public class ReviewScoreRepository : IReviewScoreRepository
    {
        private readonly ILogger<ReviewScoreRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewScoreRepository(ILogger<ReviewScoreRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            try
            {
                ReviewScore newReviewScore = new();
                newReviewScore = _mapper.Map<ReviewScore>(reviewScoreView);
                newReviewScore.CreatedAt = DateTime.UtcNow;
                newReviewScore.UpdatedAt = DateTime.UtcNow;
                var reviewScore = await _context.ReviewScores.Where(s => (s.Score == reviewScoreView.Score && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (reviewScore != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                await _context.ReviewScores.AddAsync(newReviewScore, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            try
            {
                var reviewScore = await _context.ReviewScores.Where(s => (s.Id == reviewScoreView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (reviewScore == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                reviewScore.IsDeleted = true;
                reviewScore.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteReviewScorePermanently(List<int> ids, CancellationToken cancellationToken) {
            try {
                var reviewScore = await _context.ReviewScores.Where(s => (ids.Contains(s.Id) && s.IsDeleted)).ToListAsync(cancellationToken);
                if (reviewScore.Count == 0) {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                _context.RemoveRange(reviewScore);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllDeletedReviewScore(ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken) {
            try {
                var reviewScores = from s in _context.ReviewScores
                                   orderby s.DeletedAt descending
                                   where s.IsDeleted
                                   select new DeletedReviewScoreView {
                                       Id = s.Id,
                                       Score = s.Score,
                                       Description = s.Description,
                                       DeletedAt = s.DeletedAt
                                   };
                if (!await reviewScores.AnyAsync(cancellationToken)) {
                    return new DeletedReviewScoreListView(new List<DeletedReviewScoreView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(reviewScoreParamView.SearchString)) {
                    reviewScores = reviewScores.Where(s => (s.Score.ToString().ToLower().Contains(reviewScoreParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(reviewScoreParamView.SearchString.ToLower())));
                }
                var totalRecord = await reviewScores.CountAsync(cancellationToken);
                if (reviewScoreParamView.PageIndex > 0) {
                    reviewScores = reviewScores.Skip(reviewScoreParamView.PageSize * (reviewScoreParamView.PageIndex - 1)).Take(reviewScoreParamView.PageSize);
                }
                reviewScores = reviewScores.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                return new DeletedReviewScoreListView(await reviewScores.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return new DeletedReviewScoreListView(new List<DeletedReviewScoreView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllReviewScore(ReviewScoreParamView reviewScoreParamView, CancellationToken cancellationToken)
        {
            try
            {
                var reviewScores = from s in _context.ReviewScores
                                   orderby s.Score
                                   where !s.IsDeleted
                                   select new ReviewScoreView
                                   {
                                       Id = s.Id,
                                       Score = s.Score,
                                       Description = s.Description
                                   };
                if (!await reviewScores.AnyAsync(cancellationToken))
                {
                    return new ReviewScoreListView(new List<ReviewScoreView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(reviewScoreParamView.SearchString))
                {
                    reviewScores = reviewScores.Where(s => (s.Score.ToString().ToLower().Contains(reviewScoreParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(reviewScoreParamView.SearchString.ToLower())));
                }
                var totalRecord = await reviewScores.CountAsync(cancellationToken);
                if (reviewScoreParamView.PageIndex > 0)
                {
                    reviewScores = reviewScores.Skip(reviewScoreParamView.PageSize * (reviewScoreParamView.PageIndex - 1)).Take(reviewScoreParamView.PageSize);
                }
                return new ReviewScoreListView(await reviewScores.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ReviewScoreListView(new List<ReviewScoreView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ReviewScoreView> GetReviewScoreById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var reviewScore = from s in _context.ReviewScores
                                  where s.Id == id && !s.IsDeleted
                                  select new ReviewScoreView
                                  {
                                      Id = s.Id,
                                      Description = s.Description,
                                      Score = s.Score
                                  };
                return await reviewScore.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<List<ReviewScoreView>> GetReviewScoreByListId(List<int> ids, CancellationToken cancellationToken) {
            try {
                var reviewScore = from s in _context.ReviewScores
                                  where ids.Contains(s.Id) && s.IsDeleted
                                  select new ReviewScoreView {
                                      Id = s.Id,
                                      Description = s.Description,
                                      Score = s.Score
                                  };
                return await reviewScore.ToListAsync(cancellationToken) ?? new();
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateReviewScore(ReviewScoreView reviewScoreView, CancellationToken cancellationToken)
        {
            try
            {
                var score = await _context.ReviewScores.Where(s => (s.Id == reviewScoreView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (score == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.ReviewScores.Where(s => (s.Score == reviewScoreView.Score && s.Score != score.Score && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(reviewScoreView, score);
                score.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }
    }
}
