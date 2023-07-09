using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IProducerImageRepository
    {
        public Task<Tuple<ResponseView, int, int?>> CreateProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAllProducerImage(int producerId, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAllNameProducerImage(int producerId, string newProducerName, CancellationToken cancellationToken);
    }

    public class ProducerImageRepository : IProducerImageRepository
    {
        private readonly ILogger<ProducerImageRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProducerImageRepository(ApplicationDbContext context, ILogger<ProducerImageRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Tuple<ResponseView, int, int?>> CreateProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            try
            {
                ProducerImage newProducerImage = new();
                newProducerImage = _mapper.Map<ProducerImage>(producerImageView);
                newProducerImage.CreatedAt = DateTime.UtcNow;
                newProducerImage.UpdatedAt = DateTime.UtcNow;
                await _context.ProducerImages.AddAsync(newProducerImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var id = newProducerImage.Id;
                var producer = newProducerImage.ProducerId;
                return new(new(MessageConstant.CREATE_SUCCESS, 0), id, producer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), 0, null);
            }
        }

        public async Task<ResponseView> DeleteAllProducerImage(int producerId, CancellationToken cancellationToken)
        {
            try
            {
                await _context.ProducerImages.Where(s => (s.ProducerId == producerId && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.ProducerImages.Where(s => (s.Id == producerImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
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

        public async Task<ResponseView> UpdateAllNameProducerImage(int producerId, string newProducerName, CancellationToken cancellationToken)
        {
            try
            {
                var images = await _context.ProducerImages.Where(s => (s.ProducerId == producerId && !s.IsDeleted)).ToListAsync(cancellationToken);
                foreach (var image in images)
                {
                    image.Image = $"""Image\Producer\{newProducerName}\{newProducerName}-{(image.Image ?? "").Split("-").Last()}""";
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

        public async Task<ResponseView> UpdateProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.ProducerImages.Where(s => (s.Id == producerImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (image == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                if (producerImageView.IsAvatar)
                {
                    await _context.ProducerImages.Where(s => (s.ProducerId == producerImageView.ProducerId && !s.IsDeleted && s.Id != image.Id))
                                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsAvatar, false), cancellationToken);
                }
                _mapper.Map(producerImageView, image);
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
