using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Factories;
using Lunafy.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize]
[Route("api/[area]/home")]
public class HomeApiController : ControllerBase
{
    private readonly IWorkContext _workContext;
    private readonly IHomeModelsFactory _homeModelsFactory;

    public HomeApiController(IWorkContext workContext,
        IHomeModelsFactory homeModelsFactory)
    {
        _workContext = workContext;
        _homeModelsFactory = homeModelsFactory;
    }

    [HttpGet("get-total-users")]
    [IgnoreAntiforgeryToken]
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