using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IAnimeSourceRepository
    {
        public Task<ResponseWithPaginationView> GetAllAnimeSource(AnimeSourceParamView animeSourceParamView, CancellationToken cancellationToken);
        public Task<AnimeSourceView> GetAnimeSourceById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken);
    }

    public class AnimeSourceRepository : IAnimeSourceRepository
    {
        private readonly ILogger<AnimeSourceRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AnimeSourceRepository(ILogger<AnimeSourceRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            try
            {
                var source = await _context.AnimeSources.Where(s => (s.Id == animeSourceView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (source == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeSources.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == source.Id);
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

        public async Task<ResponseView> ChangePrevOrder(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            try
            {
                var source = await _context.AnimeSources.Where(s => (s.Id == animeSourceView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (source == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeSources.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == source.Id);
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

        public async Task<ResponseView> CreateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            try
            {
                AnimeSource newAnimeSource = new();
                newAnimeSource = _mapper.Map<AnimeSource>(animeSourceView);
                newAnimeSource.CreatedAt = DateTime.UtcNow;
                newAnimeSource.UpdatedAt = DateTime.UtcNow;
                var animeSource = await _context.AnimeSources.Where(s => (s.SourceName == animeSourceView.SourceName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeSource != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                var list = await _context.AnimeSources.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var lastNav = list.OrderByDescending(s => s.Order).ThenBy(s => s.Id).FirstOrDefault();
                if (lastNav != null)
                {
                    newAnimeSource.Order = lastNav.Order + 1;
                }
                else
                {
                    newAnimeSource.Order = 1;
                }
                await _context.AnimeSources.AddAsync(newAnimeSource, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            try
            {
                var animeSource = await _context.AnimeSources.Where(s => (s.Id == animeSourceView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeSource == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                animeSource.IsDeleted = true;
                animeSource.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                var list = await _context.AnimeSources.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseWithPaginationView> GetAllAnimeSource(AnimeSourceParamView animeSourceParamView, CancellationToken cancellationToken)
        {
            try
            {
                var animeSources = from s in _context.AnimeSources
                                   orderby s.Order
                                   where !s.IsDeleted
                                   select new AnimeSourceView
                                   {
                                       Id = s.Id,
                                       SourceName = s.SourceName,
                                       Description = s.Description,
                                       Order = s.Order,
                                       AnimeCount = (from ss in _context.Animes
                                                     where ss.AnimeSourceId == s.Id && !ss.IsDeleted
                                                     select ss).Count()
                                   };
                if (!await animeSources.AnyAsync(cancellationToken))
                {
                    return new AnimeSourceListView(new List<AnimeSourceView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(animeSourceParamView.SearchString))
                {
                    animeSources = animeSources.Where(s => ((s.SourceName ?? "").ToLower().Contains(animeSourceParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(animeSourceParamView.SearchString.ToLower())));
                }
                var totalRecord = await animeSources.CountAsync(cancellationToken);
                if (animeSourceParamView.PageIndex > 0)
                {
                    animeSources = animeSources.Skip(animeSourceParamView.PageSize * (animeSourceParamView.PageIndex - 1)).Take(animeSourceParamView.PageSize);
                }
                return new AnimeSourceListView(await animeSources.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AnimeSourceListView(new List<AnimeSourceView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<AnimeSourceView> GetAnimeSourceById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var animeSource = from s in _context.AnimeSources
                                  where s.Id == id && !s.IsDeleted
                                  select new AnimeSourceView
                                  {
                                      Id = s.Id,
                                      Description = s.Description,
                                      Order = s.Order,
                                      SourceName = s.SourceName
                                  };
                return await animeSource.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateAnimeSource(AnimeSourceView animeSourceView, CancellationToken cancellationToken)
        {
            try
            {
                var source = await _context.AnimeSources.Where(s => (s.Id == animeSourceView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (source == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.AnimeSources.Where(s => (s.SourceName == animeSourceView.SourceName && s.SourceName != source.SourceName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(animeSourceView, source);
                source.UpdatedAt = DateTime.UtcNow;
                var list = await _context.AnimeSources.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        private List<AnimeSource> reOrder(List<AnimeSource> list)
        {
            for (var i = 1; i <= list.Count; i++)
            {
                list[i - 1].Order = i;
            }
            return list;
        }
    }
}
