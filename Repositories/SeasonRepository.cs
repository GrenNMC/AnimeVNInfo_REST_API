using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface ISeasonRepository
    {
        public Task<ResponseWithPaginationView> GetAllSeason(SeasonParamView seasonParamView, CancellationToken cancellationToken);
        public Task<SeasonView> GetSeasonById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateSeason(SeasonView seasonView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteSeason(SeasonView seasonView, CancellationToken cancellationToken);
    }

    public class SeasonRepository : ISeasonRepository
    {
        private readonly ILogger<SeasonRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SeasonRepository(ILogger<SeasonRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateSeason(SeasonView seasonView, CancellationToken cancellationToken)
        {
            try
            {
                Season newSeason = new();
                newSeason = _mapper.Map<Season>(seasonView);
                newSeason.CreatedAt = DateTime.UtcNow;
                newSeason.UpdatedAt = DateTime.UtcNow;
                var season = await _context.Seasons.Where(s => (s.Year == seasonView.Year && s.Quarter == seasonView.Quarter && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (season != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                await _context.Seasons.AddAsync(newSeason, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteSeason(SeasonView seasonView, CancellationToken cancellationToken)
        {
            try
            {
                var season = await _context.Seasons.Where(s => (s.Id == seasonView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (season == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                season.IsDeleted = true;
                season.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllSeason(SeasonParamView seasonParamView, CancellationToken cancellationToken)
        {
            try
            {
                var seasons = from s in _context.Seasons
                              orderby s.Year descending, s.Quarter descending
                              where !s.IsDeleted
                              select new SeasonView
                              {
                                  Id = s.Id,
                                  Quarter = s.Quarter,
                                  Year = s.Year,
                                  AnimeCount = (from ss in _context.Animes
                                                where ss.SeasonId == s.Id && !ss.IsDeleted
                                                select ss).Count()
                              };
                if (!await seasons.AnyAsync(cancellationToken))
                {
                    return new SeasonListView(new List<SeasonView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(seasonParamView.SearchString))
                {
                    seasons = seasons.Where(s => s.Year.ToString().ToLower().Contains(seasonParamView.SearchString.ToLower()));
                }
                if (seasonParamView.Order == "asc")
                {
                    if (seasonParamView.OrderBy == "animeCount")
                    {
                        seasons = seasons.OrderBy(s => s.AnimeCount).ThenBy(s => s.Id)
                                            .ThenByDescending(s => s.Year)
                                            .ThenByDescending(s => s.Quarter);
                    }
                }
                else if (seasonParamView.Order == "desc")
                {
                    if (seasonParamView.OrderBy == "animeCount")
                    {
                        seasons = seasons.OrderByDescending(s => s.AnimeCount).ThenBy(s => s.Id)
                                            .ThenByDescending(s => s.Year)
                                            .ThenByDescending(s => s.Quarter);
                    }
                }
                var totalRecord = await seasons.CountAsync(cancellationToken);
                if (seasonParamView.PageIndex > 0)
                {
                    seasons = seasons.Skip(seasonParamView.PageSize * (seasonParamView.PageIndex - 1)).Take(seasonParamView.PageSize);
                }
                return new SeasonListView(await seasons.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new SeasonListView(new List<SeasonView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<SeasonView> GetSeasonById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var season = from s in _context.Seasons
                             where s.Id == id && !s.IsDeleted
                             select new SeasonView
                             {
                                 Id = s.Id,
                                 Year = s.Year,
                                 Quarter = s.Quarter
                             };
                return await season.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }
    }
}
