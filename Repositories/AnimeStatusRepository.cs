using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IAnimeStatusRepository
    {
        public Task<ResponseWithPaginationView> GetAllAnimeStatus(AnimeStatusParamView animeStatusParamView, CancellationToken cancellationToken);
        public Task<AnimeStatusView> GetAnimeStatusById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken);
    }

    public class AnimeStatusRepository : IAnimeStatusRepository
    {
        private readonly ILogger<AnimeStatusRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AnimeStatusRepository(ILogger<AnimeStatusRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _context.AnimeStatuss.Where(s => (s.Id == animeStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (status == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == status.Id);
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

        public async Task<ResponseView> ChangePrevOrder(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _context.AnimeStatuss.Where(s => (s.Id == animeStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (status == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == status.Id);
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

        public async Task<ResponseView> CreateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            try
            {
                AnimeStatus newAnimeStatus = new();
                newAnimeStatus = _mapper.Map<AnimeStatus>(animeStatusView);
                newAnimeStatus.CreatedAt = DateTime.UtcNow;
                newAnimeStatus.UpdatedAt = DateTime.UtcNow;
                var animeStatus = await _context.AnimeStatuss.Where(s => (s.StatusName == animeStatusView.StatusName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeStatus != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                var list = await _context.AnimeStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var lastNav = list.OrderByDescending(s => s.Order).ThenBy(s => s.Id).FirstOrDefault();
                if (lastNav != null)
                {
                    newAnimeStatus.Order = lastNav.Order + 1;
                }
                else
                {
                    newAnimeStatus.Order = 1;
                }
                await _context.AnimeStatuss.AddAsync(newAnimeStatus, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var animeStatus = await _context.AnimeStatuss.Where(s => (s.Id == animeStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeStatus == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                animeStatus.IsDeleted = true;
                animeStatus.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                var list = await _context.AnimeStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseWithPaginationView> GetAllAnimeStatus(AnimeStatusParamView animeStatusParamView, CancellationToken cancellationToken)
        {
            try
            {
                var animeStatuss = from s in _context.AnimeStatuss
                                   orderby s.Order
                                   where !s.IsDeleted
                                   select new AnimeStatusView
                                   {
                                       Id = s.Id,
                                       StatusName = s.StatusName,
                                       Description = s.Description,
                                       Order = s.Order,
                                       AnimeCount = (from ss in _context.Animes
                                                     where ss.AnimeStatusId == s.Id && !ss.IsDeleted
                                                     select ss).Count()
                                   };
                if (!await animeStatuss.AnyAsync(cancellationToken))
                {
                    return new AnimeStatusListView(new List<AnimeStatusView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(animeStatusParamView.SearchString))
                {
                    animeStatuss = animeStatuss.Where(s => ((s.StatusName ?? "").ToLower().Contains(animeStatusParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(animeStatusParamView.SearchString.ToLower())));
                }
                var totalRecord = await animeStatuss.CountAsync(cancellationToken);
                if (animeStatusParamView.PageIndex > 0)
                {
                    animeStatuss = animeStatuss.Skip(animeStatusParamView.PageSize * (animeStatusParamView.PageIndex - 1)).Take(animeStatusParamView.PageSize);
                }
                return new AnimeStatusListView(await animeStatuss.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AnimeStatusListView(new List<AnimeStatusView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<AnimeStatusView> GetAnimeStatusById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var animeStatus = from s in _context.AnimeStatuss
                                  where s.Id == id && !s.IsDeleted
                                  select new AnimeStatusView
                                  {
                                      Id = s.Id,
                                      Description = s.Description,
                                      Order = s.Order,
                                      StatusName = s.StatusName
                                  };
                return await animeStatus.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateAnimeStatus(AnimeStatusView animeStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _context.AnimeStatuss.Where(s => (s.Id == animeStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (status == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.AnimeStatuss.Where(s => (s.StatusName == animeStatusView.StatusName && s.StatusName != status.StatusName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(animeStatusView, status);
                status.UpdatedAt = DateTime.UtcNow;
                var list = await _context.AnimeStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        private List<AnimeStatus> reOrder(List<AnimeStatus> list)
        {
            for (var i = 1; i <= list.Count; i++)
            {
                list[i - 1].Order = i;
            }
            return list;
        }
    }
}
