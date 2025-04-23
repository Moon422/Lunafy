using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lunafy.Api.Factories;
using Lunafy.Api.Models.Artist;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure;
using Lunafy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Lunafy.Api.Controllers;

[ApiController]
[Route("api/artist")]
public class ArtistApiController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private IArtistService _artistService;
    private readonly IArtistModelFactory _artistModelFactory;
    private readonly IWorkContext _workContext;

    public ArtistApiController(IWebHostEnvironment env,
        IMapper mapper,
        IArtistService artistService,
        IArtistModelFactory artistModelFactory,
        IWorkContext workContext)
    {
        _env = env;
        _mapper = mapper;
        _artistService = artistService;
        _artistModelFactory = artistModelFactory;
        _workContext = workContext;
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
        var directory = Directory.CreateDirectory(Path.Join(wwwRootImages, "artists", "profile", artist.Id.ToString()));

        foreach (var file in files)
        {
            System.IO.File.Copy(file, Path.Join(directory.FullName, file.Split('_').Last()));
        }

        var response = await _artistModelFactory.PrepareArtistReadModelAsync(_mapper.Map<ArtistReadModel>(artist), artist);
        return CreatedAtAction(nameof(Details), new { id = artist.Id }, response);
    }

    [HttpPut, Authorize]
    public async Task<IActionResult> Edit([FromBody] ArtistWriteModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin || await _artistService.CanBeEditedByUserAsync(model.Id, user.Id))
        {
            return Forbid();
        }

        var artist = _mapper.Map<Artist>(model);
        await _artistService.UpdateArtistAsync(artist);

        var response = _mapper.Map<ArtistReadModel>(artist);
        return Ok(response);
    }

    [HttpDelete("{artistId}"), Authorize]
    public async Task<IActionResult> Delete(int artistId)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin || !await _artistService.CanBeEditedByUserAsync(artistId, user.Id))
        {
            return Forbid();
        }

        var artist = await _artistService.GetArtistByIdAsync(artistId);
        if (artist is null)
        {
            return NotFound($"Artist with id {artistId} not found.");
        }

        await _artistService.DeleteArtistAsync(artist);
        return NoContent();
    }

    [HttpPost("upload-image"), Authorize]
    public async Task<IActionResult> UploadImage([FromBody] UploadImageModel model)
    {
        var artistId = model.ArtistId;
        var images = model.Images;
        if (artistId <= 0)
        {
            return BadRequest($"Artist is required.");
        }

        if (!images.Any())
        {
            return BadRequest("Images are required.");
        }

        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin || !await _artistService.CanBeEditedByUserAsync(artistId, user.Id))
        {
            return Forbid();
        }

        var artist = await _artistService.GetArtistByIdAsync(artistId);
        if (artist is null)
        {
            return NotFound($"Artist with id {artistId} not found.");
        }

        var uploadImagesRoot = Path.Join(_env.WebRootPath, "images", "artists", "uploads", artist.Id.ToString());
        if (!Directory.Exists(uploadImagesRoot))
        {
            Directory.CreateDirectory(uploadImagesRoot);
        }

        foreach (var image in images)
        {
            using var bitmap = SKBitmap.Decode(image.OpenReadStream());
            var skImage = SKImage.FromBitmap(bitmap);

            var filePath = Path.Join(uploadImagesRoot, $"{Guid.NewGuid():N}.webp");
            using var outputFileStream = System.IO.File.OpenWrite(filePath);

            skImage.Encode(SKEncodedImageFormat.Webp, 75).SaveTo(outputFileStream);
        }

        return Ok("Images uploaded.");
    }
}