using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AnimeVnInfoBackend.Utilities.TokenProviders
{

    public class TwoFactorCodeTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : class
    {
        public async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return await manager.GetTwoFactorEnabledAsync(user);
        }

        public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            if (!await CanGenerateTwoFactorTokenAsync(manager, user))
            {
                return "";
            }
            else
            {
                var res = "";
                for (var i = 1; i <= 6; i++)
                {
                    res += RandomNumberGenerator.GetInt32(10);
                }
                var claims = new List<Claim> { };
                claims.Add(new Claim("code", res));
                var configuration = new ConfigurationManager();
                configuration.AddJsonFile("appsettings.json");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("SecretKey") ?? ""));
                var jwtToken = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("ServerUrl"),
                    audience: configuration.GetValue<string>("ClientUrl"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                await manager.SetAuthenticationTokenAsync(user, purpose, purpose, token);
                return res;
            }
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            var configuration = new ConfigurationManager();
            configuration.AddJsonFile("appsettings.json");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("SecretKey") ?? ""));
            var code = await manager.GetAuthenticationTokenAsync(user, purpose, purpose);
            if (code != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration.GetValue<string>("ServerUrl"),
                    ValidAudience = configuration.GetValue<string>("ClientUrl"),
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };
                var x = await handler.ValidateTokenAsync(code, parameters);
                if (x.IsValid)
                {
                    var deCode = x.Claims.Where(s => s.Key == "code").FirstOrDefault();
                    if (deCode.Value != null)
                    {
                        if (deCode.Value.ToString() == token)
                        {
                            await manager.RemoveAuthenticationTokenAsync(user, purpose, purpose);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
