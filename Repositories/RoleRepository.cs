using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.ModelViews;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IRoleRepository
    {
        public Task<RoleView> GetRoleById(int id, CancellationToken cancellationToken);
        public Task<List<string>> GetListRoleByUserId(int userId, CancellationToken cancellationToken);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(ApplicationDbContext context, ILogger<RoleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<string>> GetListRoleByUserId(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var listRole = from s in _context.UserRoles
                               where s.UserId == userId
                               join r in _context.Roles on s.RoleId equals r.Id
                               select r.Name;
                return await listRole.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<RoleView> GetRoleById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var role = await (from s in _context.Roles
                                  where s.Id == id
                                  select new RoleView {
                                      Id = s.Id,
                                      Name = s.Name
                                  }).FirstOrDefaultAsync(cancellationToken);
                return role != null ? role : new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }
    }
}
