using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IStudioImageRepository
    {
        public Task<Tuple<ResponseView, int, int?>> CreateStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAllStudioImage(int studioId, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAllNameStudioImage(int studioId, string newStudioName, CancellationToken cancellationToken);
    }

    public class StudioImageRepository : IStudioImageRepository
    {
        private readonly ILogger<StudioImageRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StudioImageRepository(ApplicationDbContext context, ILogger<StudioImageRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Tuple<ResponseView, int, int?>> CreateStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            try
            {
                StudioImage newStudioImage = new();
                newStudioImage = _mapper.Map<StudioImage>(studioImageView);
                newStudioImage.CreatedAt = DateTime.UtcNow;
                newStudioImage.UpdatedAt = DateTime.UtcNow;
                await _context.StudioImages.AddAsync(newStudioImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var id = newStudioImage.Id;
                var studio = newStudioImage.StudioId;
                return new(new(MessageConstant.CREATE_SUCCESS, 0), id, studio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), 0, null);
            }
        }

        public async Task<ResponseView> DeleteAllStudioImage(int studioId, CancellationToken cancellationToken)
        {
            try
            {
                await _context.StudioImages.Where(s => (s.StudioId == studioId && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                            .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.StudioImages.Where(s => (s.Id == studioImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
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

        public async Task<ResponseView> UpdateAllNameStudioImage(int studioId, string newStudioName, CancellationToken cancellationToken)
        {
            try
            {
                var images = await _context.StudioImages.Where(s => (s.StudioId == studioId)).ToListAsync(cancellationToken);
                foreach (var image in images)
                {
                    image.Image = $"""Image\Studio\{newStudioName}\{newStudioName}-{(image.Image ?? "").Split("-").Last()}""";
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

        public async Task<ResponseView> UpdateStudioImage(StudioImageView studioImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.StudioImages.Where(s => (s.Id == studioImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (image == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                if (studioImageView.IsAvatar)
                {
                    await _context.StudioImages.Where(s => (s.StudioId == studioImageView.StudioId && !s.IsDeleted && s.Id != image.Id))
                                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsAvatar, false), cancellationToken);
                }
                _mapper.Map(studioImageView, image);
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
