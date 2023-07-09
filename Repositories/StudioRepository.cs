using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IStudioRepository
    {
        public Task<ResponseWithPaginationView> GetAllStudio(StudioParamView studioParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllStudio(StudioQueryListView studioQueryListView, CancellationToken cancellationToken);
        public Task<StudioView> GetStudioById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateStudio(StudioView studioView, CancellationToken cancellationToken);
        public Task<Tuple<ResponseView, string?>> UpdateStudio(StudioView studioView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStudio(StudioView studioView, CancellationToken cancellationToken);
    }

    public class StudioRepository : IStudioRepository
    {
        private readonly ILogger<StudioRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StudioRepository(ApplicationDbContext context, ILogger<StudioRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateStudio(StudioView studioView, CancellationToken cancellationToken)
        {
            try
            {
                var checkExisted = await _context.Studios.Where(s => (s.StudioName == studioView.StudioName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                Studio newStudio = new();
                newStudio = _mapper.Map<Studio>(studioView);
                newStudio.CreatedAt = DateTime.UtcNow;
                newStudio.UpdatedAt = DateTime.UtcNow;
                await _context.Studios.AddAsync(newStudio, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteStudio(StudioView studioView, CancellationToken cancellationToken) {
            try {
                var studio = await _context.Studios.Where(s => (s.Id == studioView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (studio == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                await _context.AnimeStudios.Where(s => (s.StudioId == studioView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                studio.IsDeleted = true;
                studio.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllStudio(StudioParamView studioParamView, CancellationToken cancellationToken)
        {
            try
            {
                var studios = from s in _context.Studios
                              orderby s.Id
                              where !s.IsDeleted
                              select new StudioView
                              {
                                  StudioName = s.StudioName,
                                  Description = s.Description,
                                  Id = s.Id,
                                  IdAnilist = s.IdAnilist,
                                  Established = s.Established,
                                  StudioLink = s.StudioLink,
                                  CreatedAt = s.CreatedAt,
                                  StudioImage = (from si in _context.StudioImages
                                                 orderby si.UpdatedAt descending
                                                 where si.StudioId == s.Id && !si.IsDeleted
                                                 select new StudioImageView
                                                 {
                                                     Id = si.Id,
                                                     StudioId = si.StudioId,
                                                     Image = si.Image,
                                                     IsAvatar = si.IsAvatar
                                                 }).ToList(),
                                  AnimeCount = (from ss in _context.AnimeStudios
                                                where ss.StudioId == s.Id && !ss.IsDeleted
                                                select ss).Count()
                              };
                if (!await studios.AnyAsync(cancellationToken))
                {
                    return new StudioListView(new List<StudioView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(studioParamView.SearchString))
                {
                    studios = studios.Where(s => ((s.StudioName != null && s.StudioName.ToLower().Contains(studioParamView.SearchString.ToLower())) ||
                                                (s.Description != null && s.Description.ToLower().Contains(studioParamView.SearchString.ToLower())) ||
                                                (s.StudioLink != null && s.StudioLink.ToLower().Contains(studioParamView.SearchString.ToLower())) ||
                                                (s.Established != null && (s.Established.ToString() ?? "").ToLower().Contains(studioParamView.SearchString.ToLower()))));
                }
                if (!string.IsNullOrWhiteSpace(studioParamView.SearchName))
                {
                    studios = studios.Where(s => (s.StudioName ?? "").ToLower().Contains(studioParamView.SearchName.ToLower()));
                }
                if (studioParamView.Order == "asc")
                {
                    if (studioParamView.OrderBy == "studioName")
                    {
                        studios = studios.OrderBy(s => s.StudioName).ThenBy(s => s.Id);
                    }
                    else if (studioParamView.OrderBy == "established")
                    {
                        studios = studios.OrderBy(s => s.Established).ThenBy(s => s.Id);
                    }
                    else if (studioParamView.OrderBy == "createdAt")
                    {
                        studios = studios.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (studioParamView.OrderBy == "animeCount")
                    {
                        studios = studios.OrderBy(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                else if (studioParamView.Order == "desc")
                {
                    if (studioParamView.OrderBy == "studioName")
                    {
                        studios = studios.OrderByDescending(s => s.StudioName).ThenBy(s => s.Id);
                    }
                    else if (studioParamView.OrderBy == "established")
                    {
                        studios = studios.OrderByDescending(s => s.Established).ThenBy(s => s.Id);
                    }
                    else if (studioParamView.OrderBy == "createdAt")
                    {
                        studios = studios.OrderByDescending(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (studioParamView.OrderBy == "animeCount")
                    {
                        studios = studios.OrderByDescending(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await studios.CountAsync(cancellationToken);
                if (studioParamView.PageIndex > 0)
                {
                    studios = studios.Skip(studioParamView.PageSize * (studioParamView.PageIndex - 1)).Take(studioParamView.PageSize);
                }
                studios = studios.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
				return new StudioListView(await studios.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new StudioListView(new List<StudioView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllStudio(StudioQueryListView studioQueryListView, CancellationToken cancellationToken)
        {
            try
            {
                if (studioQueryListView.Id == null)
                {
                    return new StudioListView(new(), 0, 2, MessageConstant.NO_DATA_REQUEST);
                }
                var studios = from s in _context.Studios
                              orderby s.Id
                              where !s.IsDeleted && studioQueryListView.Id.Contains(s.IdAnilist)
                              select new StudioView
                              {
                                  StudioName = s.StudioName,
                                  Description = s.Description,
                                  Id = s.Id,
                                  IdAnilist = s.IdAnilist,
                                  Established = s.Established,
                                  StudioLink = s.StudioLink,
                                  CreatedAt = s.CreatedAt,
                                  StudioImage = (from si in _context.StudioImages
                                                 orderby si.UpdatedAt descending
                                                 where si.StudioId == s.Id && !si.IsDeleted
                                                 select new StudioImageView
                                                 {
                                                     Id = si.Id,
                                                     StudioId = si.StudioId,
                                                     Image = si.Image,
                                                     IsAvatar = si.IsAvatar
                                                 }).ToList(),
                                  AnimeCount = (from ss in _context.AnimeStudios
                                                where ss.StudioId == s.Id && !ss.IsDeleted
                                                select ss).Count()
                              };
                var totalRecord = await studios.CountAsync(cancellationToken);
                studios = studios.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
				return new StudioListView(await studios.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new StudioListView(new List<StudioView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<StudioView> GetStudioById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var studio = from s in _context.Studios
                             where s.Id == id && !s.IsDeleted
                             select new StudioView
                             {
                                 StudioName = s.StudioName,
                                 Description = s.Description,
                                 Id = s.Id,
                                 Established = s.Established,
                                 StudioLink = s.StudioLink,
                                 CreatedAt = s.CreatedAt
                             };
                return await studio.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<Tuple<ResponseView, string?>> UpdateStudio(StudioView studioView, CancellationToken cancellationToken)
        {
            try
            {
                var studio = await _context.Studios.Where(s => (s.Id == studioView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (studio == null)
                {
                    return new(new(MessageConstant.NOT_FOUND, 2), null);
                }
                var checkExisted = await _context.Studios.Where(s => (s.StudioName == studioView.StudioName && s.StudioName != studio.StudioName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(new(MessageConstant.EXISTED, 3), null);
                }
                var name = studio.StudioName;
                _mapper.Map(studioView, studio);
                studio.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(new(MessageConstant.UPDATE_SUCCESS, 0), name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), null);
            }
        }
    }
}
