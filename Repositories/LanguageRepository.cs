using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface ILanguageRepository
    {
        public Task<ResponseWithPaginationView> GetAllLanguage(LanguageParamView LanguageParamView, CancellationToken cancellationToken);
        public Task<LanguageView> GetLanguageById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateLanguage(LanguageView LanguageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateLanguage(LanguageView LanguageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteLanguage(LanguageView LanguageView, CancellationToken cancellationToken);
    }

    public class LanguageRepository : ILanguageRepository
    {
        private readonly ILogger<LanguageRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LanguageRepository(ILogger<LanguageRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateLanguage(LanguageView LanguageView, CancellationToken cancellationToken)
        {
            try
            {
                Language newLanguage = new();
                newLanguage = _mapper.Map<Language>(LanguageView);
                newLanguage.CreatedAt = DateTime.UtcNow;
                newLanguage.UpdatedAt = DateTime.UtcNow;
                var Language = await _context.Languages.Where(s => (s.LanguageName == LanguageView.LanguageName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (Language != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                await _context.Languages.AddAsync(newLanguage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteLanguage(LanguageView LanguageView, CancellationToken cancellationToken)
        {
            try
            {
                var Language = await _context.Languages.Where(s => (s.Id == LanguageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (Language == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                Language.IsDeleted = true;
                Language.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllLanguage(LanguageParamView LanguageParamView, CancellationToken cancellationToken)
        {
            try
            {
                var Languages = from s in _context.Languages
                                orderby s.Id
                                where !s.IsDeleted
                                select new LanguageView
                                {
                                    Id = s.Id,
                                    LanguageName = s.LanguageName,
                                    Description = s.Description,
                                    OriginalName = s.OriginalName,
                                    StaffCount = (from ss in _context.Staffs
                                                  where ss.LanguageId == s.Id && !ss.IsDeleted
                                                  select ss).Count()
                                };
                if (!await Languages.AnyAsync(cancellationToken))
                {
                    return new LanguageListView(new List<LanguageView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(LanguageParamView.SearchString))
                {
                    Languages = Languages.Where(s => ((s.LanguageName ?? "").ToLower().Contains(LanguageParamView.SearchString.ToLower()) ||
                                                (s.Description ?? "").ToLower().Contains(LanguageParamView.SearchString.ToLower())));
                }
                if (LanguageParamView.Order == "asc")
                {
                    if (LanguageParamView.OrderBy == "languageName")
                    {
                        Languages = Languages.OrderBy(s => s.LanguageName).ThenBy(s => s.Id);
                    }
                    else if (LanguageParamView.OrderBy == "staffCount")
                    {
                        Languages = Languages.OrderBy(s => s.StaffCount).ThenBy(s => s.Id);
                    }
                }
                else if (LanguageParamView.Order == "desc")
                {
                    if (LanguageParamView.OrderBy == "languageName")
                    {
                        Languages = Languages.OrderByDescending(s => s.LanguageName).ThenBy(s => s.Id);
                    }
                    else if (LanguageParamView.OrderBy == "staffCount")
                    {
                        Languages = Languages.OrderByDescending(s => s.StaffCount).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await Languages.CountAsync(cancellationToken);
                if (LanguageParamView.PageIndex > 0)
                {
                    Languages = Languages.Skip(LanguageParamView.PageSize * (LanguageParamView.PageIndex - 1)).Take(LanguageParamView.PageSize);
                }
                return new LanguageListView(await Languages.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new LanguageListView(new List<LanguageView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<LanguageView> GetLanguageById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var Language = from s in _context.Languages
                               where s.Id == id && !s.IsDeleted
                               select new LanguageView
                               {
                                   Id = s.Id,
                                   Description = s.Description,
                                   OriginalName = s.OriginalName,
                                   LanguageName = s.LanguageName
                               };
                return await Language.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateLanguage(LanguageView LanguageView, CancellationToken cancellationToken)
        {
            try
            {
                var score = await _context.Languages.Where(s => (s.Id == LanguageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (score == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.Languages.Where(s => (s.LanguageName == LanguageView.LanguageName && s.LanguageName != score.LanguageName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(LanguageView, score);
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
