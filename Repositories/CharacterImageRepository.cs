using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface ICharacterImageRepository
    {
        public Task<Tuple<ResponseView, int, int?>> CreateCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAllCharacterImage(int characterId, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAllNameCharacterImage(int characterId, string newCharacterName, CancellationToken cancellationToken);
    }

    public class CharacterImageRepository : ICharacterImageRepository
    {
        private readonly ILogger<CharacterImageRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CharacterImageRepository(ApplicationDbContext context, ILogger<CharacterImageRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Tuple<ResponseView, int, int?>> CreateCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            try
            {
                CharacterImage newCharacterImage = new();
                newCharacterImage = _mapper.Map<CharacterImage>(characterImageView);
                newCharacterImage.CreatedAt = DateTime.UtcNow;
                newCharacterImage.UpdatedAt = DateTime.UtcNow;
                await _context.CharacterImages.AddAsync(newCharacterImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var id = newCharacterImage.Id;
                var character = newCharacterImage.CharacterId;
                return new(new(MessageConstant.CREATE_SUCCESS, 0), id, character);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), 0, null);
            }
        }

        public async Task<ResponseView> DeleteAllCharacterImage(int characterId, CancellationToken cancellationToken)
        {
            try
            {
                await _context.CharacterImages.Where(s => (s.CharacterId == characterId && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                    .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.CharacterImages.Where(s => (s.Id == characterImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
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

        public async Task<ResponseView> UpdateAllNameCharacterImage(int characterId, string newCharacterName, CancellationToken cancellationToken)
        {
            try
            {
                var images = await _context.CharacterImages.Where(s => (s.CharacterId == characterId)).ToListAsync(cancellationToken);
                foreach (var image in images)
                {
                    image.Image = $"""Image\Character\{newCharacterName}-{characterId}\{newCharacterName}-{characterId}-{(image.Image ?? "").Split("-").Last()}""";
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

        public async Task<ResponseView> UpdateCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.CharacterImages.Where(s => (s.Id == characterImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (image == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                if (characterImageView.IsAvatar)
                {
                    await _context.CharacterImages.Where(s => (s.CharacterId == characterImageView.CharacterId && !s.IsDeleted && s.Id != image.Id))
                                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsAvatar, false), cancellationToken);
                }
                _mapper.Map(characterImageView, image);
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
