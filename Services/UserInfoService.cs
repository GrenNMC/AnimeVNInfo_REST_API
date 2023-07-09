using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IUserInfoService
    {
        public Task<ResponseView> UpdateUserInfo(UserInfoView userInfoView, CancellationToken cancellationToken);
    }

    public class UserInfoService : IUserInfoService
    {
        private readonly IUserInfoRepository _repo;

        public UserInfoService(IUserInfoRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> UpdateUserInfo(UserInfoView userInfoView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateUserInfo(userInfoView, cancellationToken);
        }
    }
}
