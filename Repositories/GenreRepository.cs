using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IGenreRepository
    {
        public Task<ResponseWithPaginationView> GetAllGenre(GenreParamView genreParamView, CancellationToken cancellationToken);
        public Task<GenreView> GetGenreById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateGenre(GenreView genreView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateGenre(GenreView genreView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteGenre(GenreView genreView, CancellationToken cancellationToken);
    }

    public class GenreRepository : IGenreRepository
    {
        private readonly ILogger<GenreRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenreRepository(ILogger<GenreRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateGenre(GenreView genreView, CancellationToken cancellationToken)
        {
            try
            {
                Genre newGenre = new();
                newGenre = _mapper.Map<Genre>(genreView);
                newGenre.CreatedAt = DateTime.UtcNow;
                newGenre.UpdatedAt = DateTime.UtcNow;
                var genre = await _context.Genres.Where(s => (s.GenreName == genreView.GenreName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (genre != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                await _context.Genres.AddAsync(newGenre, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteGenre(GenreView genreView, CancellationToken cancellationToken)
        {
            try
            {
                var genre = await _context.Genres.Where(s => (s.Id == genreView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (genre == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                await _context.AnimeGenres.Where(s => (s.GenreId == genre.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                        .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                genre.IsDeleted = true;
                genre.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllGenre(GenreParamView genreParamView, CancellationToken cancellationToken)
        {
            try
            {
                var genres = from s in _context.Genres
                             orderby s.Id
                             where !s.IsDeleted
                             select new GenreView
                             {
                                 Id = s.Id,
                                 GenreName = s.GenreName,
                                 Description = s.Description,
                                 AnimeCount = (from ss in _context.AnimeGenres
                                               where ss.GenreId == s.Id && !ss.IsDeleted
                                               select ss).Count()
                             };
                if (!await genres.AnyAsync(cancellationToken))
                {
                    return new GenreListView(new List<GenreView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(genreParamView.SearchString))
                {
                    genres = genres.Where(s => ((s.GenreName ?? "").ToLower().Contains(genreParamView.SearchString.ToLower()) ||
                                                (s.Description ?? "").ToLower().Contains(genreParamView.SearchString.ToLower())));
                }
                if (genreParamView.Order == "asc")
                {
                    if (genreParamView.OrderBy == "genreName")
                    {
                        genres = genres.OrderBy(s => s.GenreName).ThenBy(s => s.Id);
                    }
                    else if (genreParamView.OrderBy == "animeCount")
                    {
                        genres = genres.OrderBy(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                else if (genreParamView.Order == "desc")
                {
                    if (genreParamView.OrderBy == "genreName")
                    {
                        genres = genres.OrderByDescending(s => s.GenreName).ThenBy(s => s.Id);
                    }
                    else if (genreParamView.OrderBy == "animeCount")
                    {
                        genres = genres.OrderByDescending(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await genres.CountAsync(cancellationToken);
                if (genreParamView.PageIndex > 0)
                {
                    genres = genres.Skip(genreParamView.PageSize * (genreParamView.PageIndex - 1)).Take(genreParamView.PageSize);
                }
                return new GenreListView(await genres.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GenreListView(new List<GenreView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<GenreView> GetGenreById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var genre = from s in _context.Genres
                            where s.Id == id && !s.IsDeleted
                            select new GenreView
                            {
                                Id = s.Id,
                                Description = s.Description,
                                GenreName = s.GenreName
                            };
                return await genre.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateGenre(GenreView genreView, CancellationToken cancellationToken)
        {
            try
            {
                var genre = await _context.Genres.Where(s => (s.Id == genreView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (genre == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.Genres.Where(s => (s.GenreName == genreView.GenreName && s.GenreName != genre.GenreName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(genreView, genre);
                genre.UpdatedAt = DateTime.UtcNow;
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
