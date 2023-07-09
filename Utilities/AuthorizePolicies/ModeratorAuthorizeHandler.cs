using AnimeVnInfoBackend.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace AnimeVnInfoBackend.Utilities.AuthorizePolicies
{
    public class ModeratorAuthorize : AuthorizeAttribute
    {
        public ModeratorAuthorize()
        {
            Policy = "Role.Moderator";
        }
    }

    public class ModeratorAuthorizeRequirement : IAuthorizationRequirement
    {
        public ModeratorAuthorizeRequirement() { }
    }

    public class ModeratorAuthorizeHandler : AuthorizationHandler<ModeratorAuthorizeRequirement>
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;

        public ModeratorAuthorizeHandler(IUserRepository userRepo, IRoleRepository roleRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModeratorAuthorizeRequirement requirement)
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
                    if (listRole.Contains("Moderator"))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
