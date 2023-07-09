using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IRatingRepository
    {
        public Task<ResponseWithPaginationView> GetAllRating(RatingParamView ratingParamView, CancellationToken cancellationToken);
        public Task<RatingView> GetRatingById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateRating(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateRating(RatingView ratingView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteRating(RatingView ratingView, CancellationToken cancellationToken);
    }

    public class RatingRepository : IRatingRepository
    {
        private readonly ILogger<RatingRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RatingRepository(ILogger<RatingRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> ChangeNextOrder(RatingView ratingView, CancellationToken cancellationToken)
        {
            try
            {
                var rating = await _context.Ratings.Where(s => (s.Id == ratingView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (rating == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.Ratings.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == rating.Id);
                if (index >= list.Count - 1)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                list[index].Order++;
                list[index + 1].Order--;
                list[index].UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> ChangePrevOrder(RatingView ratingView, CancellationToken cancellationToken)
        {
            try
            {
                var rating = await _context.Ratings.Where(s => (s.Id == ratingView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (rating == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.Ratings.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == rating.Id);
                if (index <= 0)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                list[index].Order--;
                list[index - 1].Order++;
                list[index].UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> CreateRating(RatingView ratingView, CancellationToken cancellationToken)
        {
            try
            {
                Rating newRating = new();
                newRating = _mapper.Map<Rating>(ratingView);
                newRating.CreatedAt = DateTime.UtcNow;
                newRating.UpdatedAt = DateTime.UtcNow;
                var rating = await _context.Ratings.Where(s => (s.RatingName == ratingView.RatingName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (rating != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                var list = await _context.Ratings.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var lastNav = list.OrderByDescending(s => s.Order).ThenBy(s => s.Id).FirstOrDefault();
                if (lastNav != null)
                {
                    newRating.Order = lastNav.Order + 1;
                }
                else
                {
                    newRating.Order = 1;
                }
                await _context.Ratings.AddAsync(newRating, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteRating(RatingView ratingView, CancellationToken cancellationToken)
        {
            try
            {
                var rating = await _context.Ratings.Where(s => (s.Id == ratingView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (rating == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                rating.IsDeleted = true;
                rating.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                var list = await _context.Ratings.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllRating(RatingParamView ratingParamView, CancellationToken cancellationToken)
        {
            try
            {
                var ratings = from s in _context.Ratings
                              orderby s.Order
                              where !s.IsDeleted
                              select new RatingView
                              {
                                  Id = s.Id,
                                  RatingName = s.RatingName,
                                  Description = s.Description,
                                  Order = s.Order,
                                  AnimeCount = (from ss in _context.Animes
                                                where ss.RatingId == s.Id && !ss.IsDeleted
                                                select ss).Count()
                              };
                if (!await ratings.AnyAsync(cancellationToken))
                {
                    return new RatingListView(new List<RatingView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(ratingParamView.SearchString))
                {
                    ratings = ratings.Where(s => ((s.RatingName ?? "").ToLower().Contains(ratingParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(ratingParamView.SearchString.ToLower())));
                }
                var totalRecord = await ratings.CountAsync(cancellationToken);
                if (ratingParamView.PageIndex > 0)
                {
                    ratings = ratings.Skip(ratingParamView.PageSize * (ratingParamView.PageIndex - 1)).Take(ratingParamView.PageSize);
                }
                return new RatingListView(await ratings.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new RatingListView(new List<RatingView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<RatingView> GetRatingById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var rating = from s in _context.Ratings
                             where s.Id == id && !s.IsDeleted
                             select new RatingView
                             {
                                 Id = s.Id,
                                 Description = s.Description,
                                 Order = s.Order,
                                 RatingName = s.RatingName
                             };
                return await rating.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateRating(RatingView ratingView, CancellationToken cancellationToken)
        {
            try
            {
                var rating = await _context.Ratings.Where(s => (s.Id == ratingView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (rating == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.Ratings.Where(s => (s.RatingName == ratingView.RatingName && s.RatingName != rating.RatingName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(ratingView, rating);
                rating.UpdatedAt = DateTime.UtcNow;
                var list = await _context.Ratings.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        private List<Rating> reOrder(List<Rating> list)
        {
            for (var i = 1; i <= list.Count; i++)
            {
                list[i - 1].Order = i;
            }
            return list;
        }
    }
}
