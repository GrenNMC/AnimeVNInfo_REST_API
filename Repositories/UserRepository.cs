using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IUserRepository
    {
        public Task<ResponseWithPaginationView> GetAllUser(UserParamView userParamView, CancellationToken cancellationToken);
        public Task<UserView> GetUserById(int id, CancellationToken cancellationToken);
        public Task<UserView> GetUserByUserName(string userName, CancellationToken cancellationToken);
        public Task<UserRolesAndDisableStatusView> GetRolesAndDisableStatus(UserView userView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateUser(UserView userView, CancellationToken cancellationToken);
        public Task<ResponseLoginView> AdminLogin(LoginView loginView, CancellationToken cancellationToken);
        public Task<ResponseLoginView> Login(LoginView loginView, CancellationToken cancellationToken);
        public Task<User?> GetUser(string username, CancellationToken cancellationToken);
        public Task<User?> GetUser(int id, CancellationToken cancellationToken);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger, UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ResponseLoginView> AdminLogin(LoginView loginView, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.Where(s => s.UserName == loginView.UserName).FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    return new(MessageConstant.USER_NOT_FOUND, 2);
                }
                if (user.IsDisabled)
                {
                    return new(MessageConstant.ACCOUNT_DISABLED, 7);
                }
                if (!user.EmailConfirmed)
                {
                    return new(MessageConstant.EMAIL_NOT_CONFIRMED, 3);
                }
                if (!user.TwoFactorEnabled)
                {
                    return new(MessageConstant.TWO_FACTOR_DISABLED, 4);
                }
                var userRoles = from s in _context.UserRoles
                                where s.UserId == user.Id
                                select s.RoleId;
                if (!(await userRoles.ToListAsync(cancellationToken)).Contains(2))
                {
                    return new(MessageConstant.IS_NOT_ADMIN, 5);
                }
                var checkHash = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash ?? "", loginView.Password ?? "");
                if (checkHash == PasswordVerificationResult.Failed)
                {
                    return new(MessageConstant.WRONG_PASSWORD, 6);
                }
                return new(user.Id, null, MessageConstant.LOGIN_SUCCESS, 0, user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllUser(UserParamView userParamView, CancellationToken cancellationToken)
        {
            try
            {
                var users = from s in _context.Users
                            select new UserView
                            {
                                CreatedAt = s.CreatedAt,
                                Email = s.Email,
                                Id = s.Id,
                                UserName = s.UserName,
                                EmailConfirmed = s.EmailConfirmed
                            };
                if (!await users.AnyAsync(cancellationToken))
                {
                    return new UserListView(new List<UserView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                var totalRecord = await users.CountAsync(cancellationToken);
                if (userParamView.PageIndex > 0)
                {
                    users = users.Skip(userParamView.PageSize * (userParamView.PageIndex - 1)).Take(userParamView.PageSize);
                }
                users = users.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
				return new UserListView(await users.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new UserListView(new List<UserView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<UserRolesAndDisableStatusView> GetRolesAndDisableStatus(UserView userView, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.Where(s => s.Id == userView.Id).FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    return new UserRolesAndDisableStatusView();
                }
                var adminRole = await _context.UserRoles.Where(s => s.UserId == userView.Id && s.RoleId == 1).FirstOrDefaultAsync(cancellationToken);
                var moderatorRole = await _context.UserRoles.Where(s => s.UserId == userView.Id && s.RoleId == 2).FirstOrDefaultAsync(cancellationToken);
                var userRole = await _context.UserRoles.Where(s => s.UserId == userView.Id && s.RoleId == 3).FirstOrDefaultAsync(cancellationToken);
                return new UserRolesAndDisableStatusView
                {
                    IsDisabled = user.IsDisabled,
                    AdminRole = adminRole != null ? true : false,
                    ModeratorRole = moderatorRole != null ? true : false,
                    UserRole = userRole != null ? true : false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new UserRolesAndDisableStatusView();
            }
        }

        public async Task<User?> GetUser(string username, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Users.Where(s => s.UserName == username).FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<User?> GetUser(int id, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Users.Where(s => s.Id == id).FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<UserView> GetUserById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await (from s in _context.Users
                                  where s.Id == id
                                  select new UserView {
                                      Id = s.Id,
                                      CreatedAt = s.CreatedAt,
                                      UserName = s.UserName,
                                      Email = s.Email,
                                      EmailConfirmed = s.EmailConfirmed
                                  }).FirstOrDefaultAsync(cancellationToken);
                return user != null ? user : new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<UserView> GetUserByUserName(string userName, CancellationToken cancellationToken)
        {
            try
            {
                var user = await (from s in _context.Users
                                  where s.UserName == userName
                                  select new UserView {
                                      Id = s.Id,
                                      CreatedAt = s.CreatedAt,
                                      UserName = s.UserName,
                                      Email = s.Email,
                                      EmailConfirmed = s.EmailConfirmed
                                  }).FirstOrDefaultAsync(cancellationToken);
                return user != null ? user : new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseLoginView> Login(LoginView loginView, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.Where(s => s.UserName == loginView.UserName).FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkHash = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash ?? "", loginView.Password ?? "");
                if (checkHash == PasswordVerificationResult.Failed)
                {
                    return new(MessageConstant.WRONG_PASSWORD, 3);
                }
                return new(user.Id, null, MessageConstant.LOGIN_SUCCESS, 0, user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> UpdateUser(UserView userView, CancellationToken cancellationToken)
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
