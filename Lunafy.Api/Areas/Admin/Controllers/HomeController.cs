using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Factories;
using Lunafy.Core.Infrastructure;
using Lunafy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize]
[Route("api/[area]/home")]
public class HomeApiController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IWorkContext _workContext;
    private readonly IHomeModelsFactory _homeModelsFactory;

    public HomeApiController(IUserService userService,
        IWorkContext workContext,
        IHomeModelsFactory homeModelsFactory)
    {
        _userService = userService;
        _workContext = workContext;
        _homeModelsFactory = homeModelsFactory;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTotalUsers()
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var model = await _homeModelsFactory.PrepareTotalUsersStatModelAsync();
        return Ok(model);
    }
}