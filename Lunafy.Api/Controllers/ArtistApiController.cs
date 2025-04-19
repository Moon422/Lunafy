using System.Security.AccessControl;
using System.Threading.Tasks;
using AutoMapper;
using Lunafy.Api.Factories;
using Lunafy.Api.Models.Artist;
using Lunafy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Services;

namespace Lunafy.Api.Controllers;

[ApiController]
[Route("api/artist")]
public class ArtistApiController : ControllerBase
{
    private readonly IMapper _mapper;
    private IArtistService _artistService;
    private readonly IArtistModelFactory _artistModelFactory;

    public ArtistApiController(IMapper mapper,
        IArtistService artistService,
        IArtistModelFactory artistModelFactory)
    {
        _mapper = mapper;
        _artistService = artistService;
        _artistModelFactory = artistModelFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ArtistSearchCommand command)
    {
        int.Clamp(command.PageNumber, 1, int.MaxValue);
        int.Clamp(command.PageSize, 1, int.MaxValue);

        var searchResult = await _artistModelFactory.PrepareArtistReadSearchResultAsync(command);

        return Ok(searchResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var artist = await _artistService.GetArtistByIdAsync(id);
        if (artist is null)
        {
            return NotFound($"Artist with id {id} not found.");
        }

        var model = await _artistModelFactory.PrepareArtistReadModelAsync(_mapper.Map<ArtistReadModel>(artist), artist);

        return Ok(model);
    }
}