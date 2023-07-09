using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AdminNavigationController : ControllerBase
    {
        private readonly IAdminNavigationService _service;
        private readonly ILoggerService _logService;

        public AdminNavigationController(IAdminNavigationService service, ILoggerService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ResponseView> CreateAdminNav([FromBody] AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            var res = await _service.CreateAdminNav(AdminNavView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã tạo danh mục admin mới {AdminNavView.Title ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangeNextOrder([FromBody] AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            var oldNav = await _service.GetAdminNavById(AdminNavView.Id, cancellationToken);
            var res = await _service.ChangeNextOrder(AdminNavView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var newNav = await _service.GetAdminNavById(AdminNavView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa danh mục admin {newNav.Title} từ thứ tự {oldNav.Order} về thứ tự {newNav.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> ChangePrevOrder([FromBody] AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            var oldNav = await _service.GetAdminNavById(AdminNavView.Id, cancellationToken);
            var res = await _service.ChangePrevOrder(AdminNavView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    var newNav = await _service.GetAdminNavById(AdminNavView.Id, cancellationToken);
                    await _logService.CreateLog($"{userName.Value} đã đưa danh mục admin {newNav.Title} từ thứ tự {oldNav.Order} về thứ tự {newNav.Order}", cancellationToken);
                }
            }
            return res;
        }

        [HttpDelete]
        [AdminAuthorize]
        public async Task<ResponseView> DeleteAdminNav([FromQuery] AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            var nav = await _service.GetAdminNavById(AdminNavView.Id, cancellationToken);
            var res = await _service.DeleteAdminNav(AdminNavView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    await _logService.CreateLog($"{userName.Value} đã xóa danh mục admin {nav.Title ?? ""}", cancellationToken);
                }
            }
            return res;
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<AdminNavigationView> GetAdminNavByLink([FromQuery] string link, CancellationToken cancellationToken)
        {
            return await _service.GetAdminNavByLink(link, cancellationToken);
        }

        [HttpGet]
        [ModeratorAuthorize]
        public async Task<ResponseWithPaginationView> GetAllAdminNav([FromQuery] AdminNavParamView adminNavParamView, CancellationToken cancellationToken)
        {
            return await _service.GetAllAdminNav(adminNavParamView, cancellationToken);
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<ResponseView> UpdateAdminNav([FromBody] AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            var nav = await _service.GetAdminNavById(AdminNavView.Id, cancellationToken);
            var res = await _service.UpdateAdminNav(AdminNavView, cancellationToken);
            if (res.ErrorCode == 0)
            {
                var userName = HttpContext.User.Claims.Where(s => s.Type == "userName").FirstOrDefault();
                if (userName != null)
                {
                    if (nav.IsDisabled == AdminNavView.IsDisabled)
                    {
                        if (nav.Title == AdminNavView.Title)
                        {
                            await _logService.CreateLog($"{userName.Value} đã cập nhật danh mục admin {nav.Title ?? ""}", cancellationToken);
                        }
                        else
                        {
                            await _logService.CreateLog($"{userName.Value} đã cập nhật danh mục admin {nav.Title ?? ""} thành {AdminNavView.Title ?? ""}", cancellationToken);
                        }
                    }
                    else if (nav.IsDisabled && !AdminNavView.IsDisabled)
                    {
                        await _logService.CreateLog($"{userName.Value} đã kích hoạt danh mục admin {AdminNavView.Title ?? ""}", cancellationToken);
                    }
                    else if (!nav.IsDisabled && AdminNavView.IsDisabled)
                    {
                        await _logService.CreateLog($"{userName.Value} đã vô hiệu hóa danh mục admin {AdminNavView.Title ?? ""}", cancellationToken);
                    }
                }
            }
            return res;
        }
    }
}
