using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IUserService
    {
        public Task<ResponseWithPaginationView> GetAllUser(UserParamView userParamView, CancellationToken cancellationToken);
        public Task<UserView> GetUserByUserName(string userName, CancellationToken cancellationToken);
        public Task<UserRolesAndDisableStatusView> GetRolesAndDisableStatus(UserView userView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateUser(UserView userView, CancellationToken cancellationToken);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseWithPaginationView> GetAllUser(UserParamView userParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllUser(userParamView, cancellationToken);
        }

        public async Task<UserRolesAndDisableStatusView> GetRolesAndDisableStatus(UserView userView, CancellationToken cancellationToken)
        {
            return await _repo.GetRolesAndDisableStatus(userView, cancellationToken);
        }

        public async Task<UserView> GetUserByUserName(string userName, CancellationToken cancellationToken)
        {
            return await _repo.GetUserByUserName(userName, cancellationToken);
        }

        public async Task<ResponseView> UpdateUser(UserView userView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateUser(userView, cancellationToken);
        }
    }
}
