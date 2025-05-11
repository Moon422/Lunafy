using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Factories;
using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure;
using Lunafy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Lunafy.Api.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize]
[Route("api/[area]/artist")]
public class ArtistApiController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IArtistService _artistService;
    private readonly IArtistModelsFactory _artistModelsFactory;
    private readonly IWorkContext _workContext;

    public ArtistApiController(IWebHostEnvironment env,
        IArtistService artistService,
        IArtistModelsFactory artistModelsFactory,
        IWorkContext workContext)
    {
        _env = env;
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

        var searchResult = await _artistModelsFactory.PrepareArtistSearchResultAsync(command);
        var response = new HttpResponseModel<SearchResultModel<ArtistModel>>
        {
            Data = searchResult
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var response = new HttpResponseModel<ArtistModel>();
        if (id <= 0)
        {
            response.Errors.Add("Artist Id cannot be less than zero. Provide a valid user Id.");
            return BadRequest(response);
        }

        var artistEntity = await _artistService.GetArtistByIdAsync(id);
        if (artistEntity is null)
        {
            response.Errors.Add($"Artist with Id {id} not found.");
            return NotFound(response);
        }

        var model = await _artistModelsFactory.PrepareArtistModelAsync(artistEntity.ToModel(), artistEntity);
        response.Data = model;
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ArtistModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }
        var response = new HttpResponseModel<ArtistModel>();
        if (model.MusicBrainzId.HasValue && await _artistService.GetArtistByMusicBrainzIdAsync(model.MusicBrainzId.Value) is not null)
        {
            response.Errors.Add("Artist with same musicbrainz Id already exists.");
            return BadRequest(response);
        }

        var artistEntity = model.ToEntity();
        await _artistService.CreateArtistAsync(artistEntity);

        var wwwRootImages = Path.Join(_env.WebRootPath, "images");
        var files = Directory.GetFiles(wwwRootImages, "no_image_*.webp");
        var directory = Directory.CreateDirectory(Path.Join(wwwRootImages, "artists", "profile", artistEntity.Id.ToString()));

        foreach (var file in files)
        {
            System.IO.File.Copy(file, Path.Join(directory.FullName, file.Split('_').Last()));
        }

        var artistModel = await _artistModelsFactory.PrepareArtistModelAsync(artistEntity.ToModel(), artistEntity);
        response.Data = artistModel;

        return CreatedAtAction(nameof(Get), new { id = artistEntity.Id }, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ArtistModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var response = new HttpResponseModel<ArtistModel>();
        if (model.Id <= 0)
        {
            response.Errors.Add("Artist Id is required.");
            return BadRequest(response);
        }

        Artist? entity;
        if (model.MusicBrainzId.HasValue && (entity = await _artistService.GetArtistByMusicBrainzIdAsync(model.MusicBrainzId.Value)) is not null && entity.Id != model.Id)
        {
            response.Errors.Add("Artist with same musicbrainz Id already exists.");
            return BadRequest(response);
        }

        entity = await _artistService.GetArtistByIdAsync(model.Id);
        if (entity is null)
        {
            return BadRequest("Artist not found.");
        }

        entity = model.UpdateEntity(entity);
        await _artistService.UpdateArtistAsync(entity);

        model = await _artistModelsFactory.PrepareArtistModelAsync(entity.ToModel(), entity);
        response.Data = model;

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        Artist? artistEntity;
        var response = new HttpResponseModel();
        if (id <= 0 || (artistEntity = await _artistService.GetArtistByIdAsync(id)) is null)
        {
            response.Errors.Add("Invalid artist selected. Please try again.");
            return BadRequest(response);
        }

        await _artistService.DeleteArtistAsync(artistEntity);
        return NoContent();
    }

    [HttpGet("musicbrainz-id-availability")]
    public async Task<IActionResult> CheckMusicbrainIdAvailability([FromQuery] Guid musicBrainzId, [FromQuery] int? artistId = null)
    {
        var artist = await _artistService.GetArtistByMusicBrainzIdAsync(musicBrainzId);
        return Ok(artist is null || (artistId.HasValue && artist.Id == artistId));
    }

    [HttpPost("{artistId}/upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(int artistId, [FromForm] IFormFile image)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var artist = await _artistService.GetArtistByIdAsync(artistId);
        var response = new HttpResponseModel();
        if (artist is null)
        {
            response.Errors.Add("Artist not found.");
            return BadRequest(response);
        }

        var uploadImagesRoot = Path.Join(_env.WebRootPath, "images", "artists", "uploads", artist.Id.ToString());
        if (!Directory.Exists(uploadImagesRoot))
        {
            Directory.CreateDirectory(uploadImagesRoot);
        }

        using var bitmap = SKBitmap.Decode(image.OpenReadStream());
        var skImage = SKImage.FromBitmap(bitmap);

        var filePath = Path.Join(uploadImagesRoot, $"{Guid.NewGuid():N}.webp");
        using var outputFileStream = System.IO.File.OpenWrite(filePath);

        skImage.Encode(SKEncodedImageFormat.Webp, 75).SaveTo(outputFileStream);

        return Ok("Images uploaded.");
    }
}
