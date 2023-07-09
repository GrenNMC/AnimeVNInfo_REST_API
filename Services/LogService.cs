using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnimeVnInfoBackend.Services
{
    public interface ILogService
    {
        public Task<ResponseLoginView> Login(LoginView loginView, CancellationToken cancellationToken);
        public Task<ResponseLoginView> AdminLogin(LoginView loginView, CancellationToken cancellationToken);
    }

    public class LogService : ILogService
    {
        private readonly IUserRepository _repo;

        public LogService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseLoginView> AdminLogin(LoginView loginView, CancellationToken cancellationToken)
        {
            var res = await _repo.AdminLogin(loginView, cancellationToken);
            return GetToken(res);
        }

        public async Task<ResponseLoginView> Login(LoginView loginView, CancellationToken cancellationToken)
        {
            var res = await _repo.Login(loginView, cancellationToken);
            return GetToken(res);
        }

        private ResponseLoginView GetToken(ResponseLoginView res)
        {
            if (res.ErrorCode == 0 && !string.IsNullOrWhiteSpace(res.UserName))
            {
                var claims = new List<Claim> { };
                claims.Add(new Claim("userName", res.UserName));
                claims.Add(new Claim("userId", res.UserId.ToString()));
                var configuration = new ConfigurationManager();
                configuration.AddJsonFile("appsettings.json");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("SecretKey") ?? ""));
                var jwtToken = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("ServerUrl"),
                    audience: configuration.GetValue<string>("ClientUrl"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddMonths(3),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                return new(res.UserId, token, res.Message, res.ErrorCode, res.UserName);
            }
            return new(0, null, res.Message, res.ErrorCode, null);
        }
    }
}
