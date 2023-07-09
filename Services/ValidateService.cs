using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Emails;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AnimeVnInfoBackend.Services
{
    public interface IValidateService
    {
        public Task<ResponseView> GetCode(ValidateCodeView validateCodeView, CancellationToken cancellationToken);
        public Task<ResponseView> AdminGetCode(ValidateCodeView validateCodeView, CancellationToken cancellationToken);
        public Task<ResponseView> ValidateCode(ValidateCodeView validateCodeView, CancellationToken cancellationToken);
    }

    public class ValidateService : IValidateService
    {
        private readonly ILogger<ValidateService> _logger;
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly UserManager<User> _userManager;

        public ValidateService(ILogger<ValidateService> logger, IUserRepository userRepo, IRoleRepository roleRepo, UserManager<User> userManager)
        {
            _logger = logger;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _userManager = userManager;
        }

        public async Task<ResponseView> AdminGetCode(ValidateCodeView validateCodeView, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(validateCodeView.UserName))
                {
                    var user = await _userRepo.GetUser(validateCodeView.UserName, cancellationToken);
                    if (user != null && !string.IsNullOrWhiteSpace(user.UserName))
                    {
                        var roles = await _roleRepo.GetListRoleByUserId(user.Id, cancellationToken);
                        var adminRole = roles.Where(s => s == "Moderator").FirstOrDefault();
                        if (adminRole == null)
                        {
                            return new(MessageConstant.IS_NOT_ADMIN, 3);
                        }
                        if (user.EmailConfirmed && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "TwoFactorCode");
                            if (string.IsNullOrWhiteSpace(code))
                            {
                                return new(MessageConstant.TWO_FACTOR_DISABLED, 5);
                            }
                            var email = new TwoFactorCodeEmail();
                            email.SendMail(user.Email, code, cancellationToken);
                            return new(MessageConstant.CODE_SENT, 0);
                        }
                        else
                        {
                            return new(MessageConstant.EMAIL_NOT_CONFIRMED, 4);
                        }
                    }
                }
                return new(MessageConstant.USER_NOT_FOUND, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> GetCode(ValidateCodeView validateCodeView, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(validateCodeView.UserName))
                {
                    var user = await _userRepo.GetUser(validateCodeView.UserName, cancellationToken);
                    if (user != null && !string.IsNullOrWhiteSpace(user.UserName))
                    {
                        if (user.EmailConfirmed && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "TwoFactorCode");
                            if (string.IsNullOrWhiteSpace(code))
                            {
                                return new(MessageConstant.TWO_FACTOR_DISABLED, 4);
                            }
                            var email = new TwoFactorCodeEmail();
                            email.SendMail(user.Email, code, cancellationToken);
                            return new(MessageConstant.OK, 0);
                        }
                        else
                        {
                            return new(MessageConstant.EMAIL_NOT_CONFIRMED, 3);
                        }
                    }
                }
                return new(MessageConstant.USER_NOT_FOUND, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> ValidateCode(ValidateCodeView validateCodeView, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(validateCodeView.UserName) && !string.IsNullOrWhiteSpace(validateCodeView.Code))
                {
                    var user = await _userRepo.GetUser(validateCodeView.UserName, cancellationToken);
                    if (user != null && !string.IsNullOrWhiteSpace(user.UserName))
                    {
                        var x = await _userManager.VerifyTwoFactorTokenAsync(user, "TwoFactorCode", validateCodeView.Code);
                        if (x)
                        {
                            return new(MessageConstant.OK, 0);
                        }
                        return new(MessageConstant.VALIDATE_FAILED, 103);
                    }
                }
                return new(MessageConstant.NOT_FOUND, 102);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }
    }
}
