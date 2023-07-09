using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IAnimeImageRepository
    {
        public Task<Tuple<ResponseView, int, int?>> CreateAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAllAnimeImage(int animeId, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAllNameAnimeImage(int animeId, string newAnimeName, CancellationToken cancellationToken);
    }

    public class AnimeImageRepository : IAnimeImageRepository
    {
        private readonly ILogger<AnimeImageRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AnimeImageRepository(ApplicationDbContext context, ILogger<AnimeImageRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Tuple<ResponseView, int, int?>> CreateAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            try
            {
                AnimeImage newAnimeImage = new();
                newAnimeImage = _mapper.Map<AnimeImage>(animeImageView);
                newAnimeImage.CreatedAt = DateTime.UtcNow;
                newAnimeImage.UpdatedAt = DateTime.UtcNow;
                await _context.AnimeImages.AddAsync(newAnimeImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var id = newAnimeImage.Id;
                var anime = newAnimeImage.AnimeId;
                return new(new(MessageConstant.CREATE_SUCCESS, 0), id, anime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), 0, null);
            }
        }

        public async Task<ResponseView> DeleteAllAnimeImage(int animeId, CancellationToken cancellationToken)
        {
            try
            {
                await _context.AnimeImages.Where(s => (s.AnimeId == animeId && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                        .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.AnimeImages.Where(s => (s.Id == animeImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (image == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                image.IsDeleted = true;
                image.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> UpdateAllNameAnimeImage(int animeId, string newAnimeName, CancellationToken cancellationToken)
        {
            try
            {
                var images = await _context.AnimeImages.Where(s => (s.AnimeId == animeId)).ToListAsync(cancellationToken);
                foreach (var image in images)
                {
                    image.Image = $"""Image\Anime\{newAnimeName}-{animeId}\{newAnimeName}-{animeId}-{(image.Image ?? "").Split("-").Last()}""";
                }
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> UpdateAnimeImage(AnimeImageView animeImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.AnimeImages.Where(s => (s.Id == animeImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (image == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                if (animeImageView.IsAvatar)
                {
                    await _context.AnimeImages.Where(s => (s.AnimeId == animeImageView.AnimeId && !s.IsDeleted && s.Id != image.Id))
                                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsAvatar, false), cancellationToken);
                }
                _mapper.Map(animeImageView, image);
                image.UpdatedAt = DateTime.UtcNow;
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
