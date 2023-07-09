using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IStaffImageRepository
    {
        public Task<Tuple<ResponseView, int, int?>> CreateStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAllStaffImage(int staffId, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAllNameStaffImage(int staffId, string newStaffName, CancellationToken cancellationToken);
    }

    public class StaffImageRepository : IStaffImageRepository
    {
        private readonly ILogger<StaffImageRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StaffImageRepository(ApplicationDbContext context, ILogger<StaffImageRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Tuple<ResponseView, int, int?>> CreateStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            try
            {
                StaffImage newStaffImage = new();
                newStaffImage = _mapper.Map<StaffImage>(staffImageView);
                newStaffImage.CreatedAt = DateTime.UtcNow;
                newStaffImage.UpdatedAt = DateTime.UtcNow;
                await _context.StaffImages.AddAsync(newStaffImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var id = newStaffImage.Id;
                var staff = newStaffImage.StaffId;
                return new(new(MessageConstant.CREATE_SUCCESS, 0), id, staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), 0, null);
            }
        }

        public async Task<ResponseView> DeleteAllStaffImage(int staffId, CancellationToken cancellationToken)
        {
            try
            {
                await _context.StaffImages.Where(s => (s.StaffId == staffId && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                            .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.StaffImages.Where(s => (s.Id == staffImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
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

        public async Task<ResponseView> UpdateAllNameStaffImage(int staffId, string newStaffName, CancellationToken cancellationToken)
        {
            try
            {
                var images = await _context.StaffImages.Where(s => (s.StaffId == staffId)).ToListAsync(cancellationToken);
                foreach (var image in images)
                {
                    image.Image = $"""Image\Staff\{newStaffName}-{staffId}\{newStaffName}-{staffId}-{(image.Image ?? "").Split("-").Last()}""";
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

        public async Task<ResponseView> UpdateStaffImage(StaffImageView staffImageView, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _context.StaffImages.Where(s => (s.Id == staffImageView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (image == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                if (staffImageView.IsAvatar)
                {
                    await _context.StaffImages.Where(s => (s.StaffId == staffImageView.StaffId && !s.IsDeleted && s.Id != image.Id))
                                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsAvatar, false), cancellationToken);
                }
                _mapper.Map(staffImageView, image);
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
