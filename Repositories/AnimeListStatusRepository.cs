using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IAnimeListStatusRepository
    {
        public Task<ResponseWithPaginationView> GetAllAnimeListStatus(AnimeListStatusParamView animeListStatusParamView, CancellationToken cancellationToken);
        public Task<AnimeListStatusView> GetAnimeListStatusById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken);
    }

    public class AnimeListStatusRepository : IAnimeListStatusRepository
    {
        private readonly ILogger<AnimeListStatusRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AnimeListStatusRepository(ILogger<AnimeListStatusRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _context.AnimeListStatuss.Where(s => (s.Id == animeListStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (status == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeListStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseView> ChangePrevOrder(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _context.AnimeListStatuss.Where(s => (s.Id == animeListStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (status == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeListStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseView> CreateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            try
            {
                AnimeListStatus newAnimeListStatus = new();
                newAnimeListStatus = _mapper.Map<AnimeListStatus>(animeListStatusView);
                newAnimeListStatus.CreatedAt = DateTime.UtcNow;
                newAnimeListStatus.UpdatedAt = DateTime.UtcNow;
                var animeListStatus = await _context.AnimeListStatuss.Where(s => (s.StatusName == animeListStatusView.StatusName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeListStatus != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                var list = await _context.AnimeListStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var lastNav = list.OrderByDescending(s => s.Order).ThenBy(s => s.Id).FirstOrDefault();
                if (lastNav != null)
                {
                    newAnimeListStatus.Order = lastNav.Order + 1;
                }
                else
                {
                    newAnimeListStatus.Order = 1;
                }
                await _context.AnimeListStatuss.AddAsync(newAnimeListStatus, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var animeListStatus = await _context.AnimeListStatuss.Where(s => (s.Id == animeListStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeListStatus == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                animeListStatus.IsDeleted = true;
                animeListStatus.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                var list = await _context.AnimeListStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseWithPaginationView> GetAllAnimeListStatus(AnimeListStatusParamView animeListStatusParamView, CancellationToken cancellationToken)
        {
            try
            {
                var animeListStatuss = from s in _context.AnimeListStatuss
                                       orderby s.Order
                                       where !s.IsDeleted
                                       select new AnimeListStatusView
                                       {
                                           Id = s.Id,
                                           StatusName = s.StatusName,
                                           Description = s.Description,
                                           Color = s.Color,
                                           Order = s.Order
                                       };
                if (!await animeListStatuss.AnyAsync(cancellationToken))
                {
                    return new AnimeListStatusListView(new List<AnimeListStatusView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(animeListStatusParamView.SearchString))
                {
                    animeListStatuss = animeListStatuss.Where(s => ((s.StatusName ?? "").ToLower().Contains(animeListStatusParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(animeListStatusParamView.SearchString.ToLower())));
                }
                var totalRecord = await animeListStatuss.CountAsync(cancellationToken);
                if (animeListStatusParamView.PageIndex > 0)
                {
                    animeListStatuss = animeListStatuss.Skip(animeListStatusParamView.PageSize * (animeListStatusParamView.PageIndex - 1)).Take(animeListStatusParamView.PageSize);
                }
                return new AnimeListStatusListView(await animeListStatuss.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AnimeListStatusListView(new List<AnimeListStatusView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<AnimeListStatusView> GetAnimeListStatusById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var animeListStatus = from s in _context.AnimeListStatuss
                                      where s.Id == id && !s.IsDeleted
                                      select new AnimeListStatusView
                                      {
                                          Id = s.Id,
                                          Description = s.Description,
                                          Order = s.Order,
                                          Color = s.Color,
                                          StatusName = s.StatusName
                                      };
                return await animeListStatus.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateAnimeListStatus(AnimeListStatusView animeListStatusView, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _context.AnimeListStatuss.Where(s => (s.Id == animeListStatusView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (status == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.AnimeListStatuss.Where(s => (s.StatusName == animeListStatusView.StatusName && s.StatusName != status.StatusName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(animeListStatusView, status);
                status.UpdatedAt = DateTime.UtcNow;
                var list = await _context.AnimeListStatuss.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        private List<AnimeListStatus> reOrder(List<AnimeListStatus> list)
        {
            for (var i = 1; i <= list.Count; i++)
            {
                list[i - 1].Order = i;
            }
            return list;
        }
    }
}
