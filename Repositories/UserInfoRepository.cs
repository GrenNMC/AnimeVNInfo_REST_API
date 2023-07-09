using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IUserInfoRepository
    {
        public Task<ResponseView> UpdateUserInfo(UserInfoView userInfoView, CancellationToken cancellationToken);
    }

    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly ILogger<UserInfoRepository> _logger;
        private readonly ApplicationDbContext _context;

        public UserInfoRepository(ILogger<UserInfoRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseView> UpdateUserInfo(UserInfoView userInfoView, CancellationToken cancellationToken)
        {
            try
            {
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
