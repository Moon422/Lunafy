using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lunafy.Api.Factories;
using Lunafy.Api.Models.Artist;
using Lunafy.Core.Domains;
using Lunafy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Controllers;

[ApiController]
[Route("api/artist")]
public class ArtistApiController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private IArtistService _artistService;
    private readonly IArtistModelFactory _artistModelFactory;

    public ArtistApiController(IWebHostEnvironment env,
        IMapper mapper,
        IArtistService artistService,
        IArtistModelFactory artistModelFactory)
    {
        _env = env;
        _mapper = mapper;
        _artistService = artistService;
        _artistModelFactory = artistModelFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ArtistSearchCommand command)
    {
        Math.Clamp(command.PageNumber, 1, int.MaxValue);
        Math.Clamp(command.PageSize, 1, int.MaxValue);

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

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ArtistWriteModel model)
    {
        var artist = _mapper.Map<Artist>(model);
        await _artistService.CreateArtistAsync(artist);

        var wwwRootImages = Path.Join(_env.WebRootPath, "images");
        var files = Directory.GetFiles(wwwRootImages, "no_image_*.webp");
        var directory = Directory.CreateDirectory(Path.Join(wwwRootImages, "artists", artist.Id.ToString()));

        foreach (var file in files)
        {
            System.IO.File.Copy(file, Path.Join(directory.FullName, file.Split('_').Last()));
        }

        var response = await _artistModelFactory.PrepareArtistReadModelAsync(_mapper.Map<ArtistReadModel>(artist), artist);
        return CreatedAtAction(nameof(Details), new { id = artist.Id }, response);
    }
}