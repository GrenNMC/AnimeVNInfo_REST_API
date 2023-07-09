using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class MainNavigationController : ControllerBase
    {
        //private readonly IMainNavigationService _service;

        //public MainNavigationController(IMainNavigationService service)
        //{
        //    _service = service;
        //}

        //[HttpPost]
        //[AdminAuthorize]
        //public ResponseView CreateMainNav([FromBody] MainNavigationView MainNavView)
        //{
        //    return _service.CreateMainNav(MainNavView);
        //}

        //[HttpDelete]
        //[AdminAuthorize]
        //public ResponseView DeleteMainNav([FromQuery] MainNavigationView MainNavView)
        //{
        //    return _service.DeleteMainNav(MainNavView);
        //}

        //[HttpGet]
        //public List<MainNavigationView> GetNavigation()
        //{
        //    //return _service.GetNavigation();
        //}

        //[HttpGet]
        //[AdminAuthorize]
        //public ResponseWithPaginationView GetAllMainNav([FromQuery] MainNavParamView adminNavParamView)
        //{
        //    return _service.GetAllMainNav(adminNavParamView);
        //}

        //[HttpPut]
        //[AdminAuthorize]
        //public ResponseView UpdateMainNav([FromBody] MainNavigationView MainNavView)
        //{
        //    return _service.UpdateMainNav(MainNavView);
        //}
    }
}
