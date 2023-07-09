using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IRelationTypeRepository
    {
        public Task<ResponseWithPaginationView> GetAllRelationType(RelationTypeParamView relationTypeParamView, CancellationToken cancellationToken);
        public Task<RelationTypeView> GetRelationTypeById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken);
    }

    public class RelationTypeRepository : IRelationTypeRepository
    {
        private readonly ILogger<RelationTypeRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RelationTypeRepository(ILogger<RelationTypeRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> ChangeNextOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var type = await _context.RelationTypes.Where(s => (s.Id == relationTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (type == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.RelationTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseView> ChangePrevOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var type = await _context.RelationTypes.Where(s => (s.Id == relationTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (type == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.RelationTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseView> CreateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            try
            {
                RelationType newRelationType = new();
                newRelationType = _mapper.Map<RelationType>(relationTypeView);
                newRelationType.CreatedAt = DateTime.UtcNow;
                newRelationType.UpdatedAt = DateTime.UtcNow;
                var relationType = await _context.RelationTypes.Where(s => (s.TypeName == relationTypeView.TypeName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (relationType != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                var list = await _context.RelationTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var lastNav = list.OrderByDescending(s => s.Order).ThenBy(s => s.Id).FirstOrDefault();
                if (lastNav != null)
                {
                    newRelationType.Order = lastNav.Order + 1;
                }
                else
                {
                    newRelationType.Order = 1;
                }
                await _context.RelationTypes.AddAsync(newRelationType, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var relationType = await _context.RelationTypes.Where(s => (s.Id == relationTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (relationType == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                relationType.IsDeleted = true;
                relationType.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                var list = await _context.RelationTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        public async Task<ResponseWithPaginationView> GetAllRelationType(RelationTypeParamView relationTypeParamView, CancellationToken cancellationToken)
        {
            try
            {
                var relationTypes = from s in _context.RelationTypes
                                    orderby s.Order
                                    where !s.IsDeleted
                                    select new RelationTypeView {
                                        Id = s.Id,
                                        TypeName = s.TypeName,
                                        Description = s.Description,
                                        Order = s.Order
                                    };
                if (!await relationTypes.AnyAsync(cancellationToken))
                {
                    return new RelationTypeListView(new List<RelationTypeView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(relationTypeParamView.SearchString))
                {
                    relationTypes = relationTypes.Where(s => ((s.TypeName ?? "").ToLower().Contains(relationTypeParamView.SearchString.ToLower()) ||
                                                        (s.Description ?? "").ToLower().Contains(relationTypeParamView.SearchString.ToLower())));
                }
                var totalRecord = await relationTypes.CountAsync(cancellationToken);
                if (relationTypeParamView.PageIndex > 0)
                {
                    relationTypes = relationTypes.Skip(relationTypeParamView.PageSize * (relationTypeParamView.PageIndex - 1)).Take(relationTypeParamView.PageSize);
                }
                return new RelationTypeListView(await relationTypes.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new RelationTypeListView(new List<RelationTypeView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<RelationTypeView> GetRelationTypeById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var relationType = from s in _context.RelationTypes
                                   where s.Id == id && !s.IsDeleted
                                   select new RelationTypeView {
                                       Id = s.Id,
                                       Description = s.Description,
                                       Order = s.Order,
                                       TypeName = s.TypeName
                                   };
                return await relationType.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            try
            {
                var type = await _context.RelationTypes.Where(s => (s.Id == relationTypeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (type == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.RelationTypes.Where(s => (s.TypeName == relationTypeView.TypeName && s.TypeName != type.TypeName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(relationTypeView, type);
                type.UpdatedAt = DateTime.UtcNow;
                var list = await _context.RelationTypes.Where(s => !s.IsDeleted).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
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

        private List<RelationType> reOrder(List<RelationType> list)
        {
            for (var i = 1; i <= list.Count; i++)
            {
                list[i - 1].Order = i;
            }
            return list;
        }
    }
}
