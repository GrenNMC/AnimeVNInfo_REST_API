using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IAnimeTypeRepository
    {
        public Task<ResponseWithPaginationView> GetAllAnimeType(AnimeTypeParamView animeTypeParamView, CancellationToken cancellationToken);
        public Task<AnimeTypeView> GetAnimeTypeById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken);
    }

    public class AnimeTypeRepository : IAnimeTypeRepository
    {
        private readonly ILogger<AnimeTypeRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AnimeTypeRepository(ILogger<AnimeTypeRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> ChangeNextOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var type = await _context.AnimeTypes.Where(s => (s.Id == animeTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (type == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == type.Id);
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

        public async Task<ResponseView> ChangePrevOrder(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var type = await _context.AnimeTypes.Where(s => (s.Id == animeTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (type == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AnimeTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == type.Id);
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

        public async Task<ResponseView> CreateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            try
            {
                AnimeType newAnimeType = new();
                newAnimeType = _mapper.Map<AnimeType>(animeTypeView);
                newAnimeType.CreatedAt = DateTime.UtcNow;
                newAnimeType.UpdatedAt = DateTime.UtcNow;
                var animeType = await _context.AnimeTypes.Where(s => (s.TypeName == animeTypeView.TypeName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeType != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                var list = await _context.AnimeTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var lastNav = list.OrderByDescending(s => s.Order).ThenBy(s => s.Id).FirstOrDefault();
                if (lastNav != null)
                {
                    newAnimeType.Order = lastNav.Order + 1;
                }
                else
                {
                    newAnimeType.Order = 1;
                }
                await _context.AnimeTypes.AddAsync(newAnimeType, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var animeType = await _context.AnimeTypes.Where(s => (s.Id == animeTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (animeType == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                animeType.IsDeleted = true;
                animeType.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                var list = await _context.AnimeTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseWithPaginationView> GetAllAnimeType(AnimeTypeParamView animeTypeParamView, CancellationToken cancellationToken)
        {
            try
            {
                var animeTypes = from s in _context.AnimeTypes
                                 orderby s.Order
                                 where !s.IsDeleted
                                 select new AnimeTypeView
                                 {
                                     Id = s.Id,
                                     TypeName = s.TypeName,
                                     Description = s.Description,
                                     Order = s.Order,
                                     AnimeCount = (from ss in _context.Animes
                                                   where ss.AnimeTypeId == s.Id && !ss.IsDeleted
                                                   select ss).Count()
                                 };
                if (!await animeTypes.AnyAsync(cancellationToken))
                {
                    return new AnimeTypeListView(new List<AnimeTypeView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(animeTypeParamView.SearchString))
                {
                    animeTypes = animeTypes.Where(s => ((s.TypeName ?? "").ToLower().Contains(animeTypeParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(animeTypeParamView.SearchString.ToLower())));
                }
                var totalRecord = await animeTypes.CountAsync(cancellationToken);
                if (animeTypeParamView.PageIndex > 0)
                {
                    animeTypes = animeTypes.Skip(animeTypeParamView.PageSize * (animeTypeParamView.PageIndex - 1)).Take(animeTypeParamView.PageSize);
                }
                return new AnimeTypeListView(await animeTypes.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AnimeTypeListView(new List<AnimeTypeView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<AnimeTypeView> GetAnimeTypeById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var animeType = from s in _context.AnimeTypes
                                where s.Id == id && !s.IsDeleted
                                select new AnimeTypeView
                                {
                                    Id = s.Id,
                                    Description = s.Description,
                                    Order = s.Order,
                                    TypeName = s.TypeName
                                };
                return await animeType.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateAnimeType(AnimeTypeView animeTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var type = await _context.AnimeTypes.Where(s => (s.Id == animeTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (type == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.AnimeTypes.Where(s => (s.TypeName == animeTypeView.TypeName && s.TypeName != type.TypeName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(animeTypeView, type);
                type.UpdatedAt = DateTime.UtcNow;
                var list = await _context.AnimeTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        private List<AnimeType> reOrder(List<AnimeType> list)
        {
            for (var i = 1; i <= list.Count; i++)
            {
                list[i - 1].Order = i;
            }
            return list;
        }
    }
}
