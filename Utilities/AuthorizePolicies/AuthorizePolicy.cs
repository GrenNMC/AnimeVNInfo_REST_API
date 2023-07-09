using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AnimeVnInfoBackend.Utilities.AuthorizePolicies
{
    public class AuthorizePolicy : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider defaultPolicyProvider { get; }
        public AuthorizePolicy(IOptions<AuthorizationOptions> options)
        {
            defaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return defaultPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            string[] subStringPolicy = policyName.Split(new char[] { '.' });
            if (subStringPolicy.Length > 1 && subStringPolicy[0].Equals("Role", StringComparison.OrdinalIgnoreCase) && subStringPolicy[1].Equals("User"))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new UserAuthorizeRequirement());
                return Task.FromResult(policy?.Build());
            }
            else if (subStringPolicy.Length > 1 && subStringPolicy[0].Equals("Role", StringComparison.OrdinalIgnoreCase) && subStringPolicy[1].Equals("Moderator"))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new ModeratorAuthorizeRequirement());
                return Task.FromResult(policy?.Build());
            }
            else if (subStringPolicy.Length > 1 && subStringPolicy[0].Equals("Role", StringComparison.OrdinalIgnoreCase) && subStringPolicy[1].Equals("Admin"))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new AdminAuthorizeRequirement());
                return Task.FromResult(policy?.Build());
            }
            return defaultPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return defaultPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}
