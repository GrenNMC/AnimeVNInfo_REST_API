using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IAdminNavigationService
    {
        public Task<ResponseWithPaginationView> GetAllAdminNav(AdminNavParamView AdminNavParamView, CancellationToken cancellationToken);
        public Task<AdminNavigationView> GetAdminNavById(int id, CancellationToken cancellationToken);
        public Task<AdminNavigationView> GetAdminNavByLink(string link, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
    }

    public class AdminNavigationService : IAdminNavigationService
    {
        private readonly IAdminNavRepository _repo;

        public AdminNavigationService(IAdminNavRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> ChangeNextOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            return await _repo.ChangeNextOrder(AdminNavView, cancellationToken);
        }

        public async Task<ResponseView> ChangePrevOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            return await _repo.ChangePrevOrder(AdminNavView, cancellationToken);
        }

        public async Task<ResponseView> CreateAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            return await _repo.CreateAdminNav(AdminNavView, cancellationToken);
        }

        public async Task<ResponseView> DeleteAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAdminNav(AdminNavView, cancellationToken);
        }

        public async Task<AdminNavigationView> GetAdminNavById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetAdminNavById(id, cancellationToken);
        }

        public async Task<AdminNavigationView> GetAdminNavByLink(string link, CancellationToken cancellationToken)
        {
            return await _repo.GetAdminNavByLink(link, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllAdminNav(AdminNavParamView AdminNavParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAdminNav(AdminNavParamView, cancellationToken);
        }

        public async Task<ResponseView> UpdateAdminNav(AdminNavigationView AdminNavParamView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateAdminNav(AdminNavParamView, cancellationToken);
        }
    }
}
