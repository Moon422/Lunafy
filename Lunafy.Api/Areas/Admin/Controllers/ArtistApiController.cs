using System;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Factories;
using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Models;
using Lunafy.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize]
[Route("api/[area]/artist")]
public class ArtistApiController : ControllerBase
{
    private readonly IArtistModelsFactory _artistModelsFactory;
    private readonly IWorkContext _workContext;

    public ArtistApiController(IArtistModelsFactory artistModelsFactory,
        IWorkContext workContext)
    {
        _workContext = workContext;
        _artistModelsFactory = artistModelsFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ArtistSearchCommand command)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        Math.Clamp(command.PageNumber, 1, int.MaxValue);
        Math.Clamp(command.PageSize, 1, int.MaxValue);

        var searchResult = await _artistModelsFactory.PrepareArtistReadSearchResultAsync(command);
        var response = new HttpResponseModel<SearchResultModel<ArtistReadModel>>
        {
            Data = searchResult
        };

        return Ok(response);
    }
}
