using AnimeVnInfoBackend.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace AnimeVnInfoBackend.Utilities.AuthorizePolicies
{
    public class UserAuthorize : AuthorizeAttribute
    {
        public UserAuthorize()
        {
            Policy = "Role.User";
        }
    }

    public class UserAuthorizeRequirement : IAuthorizationRequirement
    {
        public UserAuthorizeRequirement() { }
    }

    public class UserAuthorizeHandler : AuthorizationHandler<UserAuthorizeRequirement>
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;

        public UserAuthorizeHandler(IUserRepository userRepo, IRoleRepository roleRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizeRequirement requirement)
        {
            var claims = context.User.Claims;
            var userId = claims.Where(s => s.Type == "userId").FirstOrDefault();
            var userName = claims.Where(s => s.Type == "userName").FirstOrDefault();
            if (userId != null && userName != null)
            {
                var user = await _userRepo.GetUser(int.TryParse(userId.Value, out _) ? int.Parse(userId.Value) : 0, CancellationToken.None);
                if (user != null && user.UserName == userName.Value && !user.IsDisabled)
                {
                    var listRole = await _roleRepo.GetListRoleByUserId(user.Id, CancellationToken.None);
                    if (listRole.Contains("User"))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
