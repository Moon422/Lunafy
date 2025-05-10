using System;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Factories;
using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Models;
using Lunafy.Core.Infrastructure;
using Lunafy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize]
[Route("api/[area]/artist")]
public class ArtistApiController : ControllerBase
{
    private readonly IArtistService _artistService;
    private readonly IArtistModelsFactory _artistModelsFactory;
    private readonly IWorkContext _workContext;

    public ArtistApiController(IArtistService artistService,
        IArtistModelsFactory artistModelsFactory,
        IWorkContext workContext)
    {
        _workContext = workContext;
        _artistModelsFactory = artistModelsFactory;
        _artistService = artistService;
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

    [HttpGet('musicbrainz-id-availability')]
    public async Task<IActionResult> CheckMusicbrainIdAvailability([FromQuery] Guid musicBrainzId, [FromQuery] int? artistId = null)
    {
        var artist = await _artistService.GetArtistByIdAsync(artistId)
    }
}
