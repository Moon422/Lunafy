using System;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models;
using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Microsoft.AspNetCore.Http;

namespace Lunafy.Api.Areas.Admin.Factories;

public interface IArtistModelsFactory
{
    Task<ArtistModel> PrepareArtistModelAsync(ArtistModel model, Artist artist);
    Task<SearchResultModel<ArtistModel>> PrepareArtistSearchResultAsync(ArtistSearchCommand searchCommand);
}

public interface IPictureModelFactory
{
    Task<PictureModel> PreparePictureModelAsync(PictureModel model, Picture? picture);
}

[ScopeDependency(typeof(IPictureModelFactory))]
public class PictureModelFactory : IPictureModelFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PictureModelFactory(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string PrepareNoPictureThumbUrl(int width)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext cannot be null.");

        var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/images/no_image_{width}.webp";
        return url;
    }

    private string PreparePictureThumbUrl(int artistId, string filename, int width)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext cannot be null.");

        var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/images/artists/thumbs/{artistId}/{filename}_{width}.webp";
        return url;
    }

    public async Task<PictureModel> PreparePictureModelAsync(PictureModel model, Picture? picture)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext cannot be null.");

        if (picture is null)
        {
            model.Thumb64 = PrepareNoPictureThumbUrl(64);
            model.Thumb128 = PrepareNoPictureThumbUrl(128);
            model.Thumb256 = PrepareNoPictureThumbUrl(256);
            model.Thumb512 = PrepareNoPictureThumbUrl(512);
            model.Thumb1024 = PrepareNoPictureThumbUrl(1024);

            return await Task.FromResult(model);
        }

        var entityType = (PictureEntityType)picture.PictureEntityTypeId switch
        {
            PictureEntityType.User => "users",
            PictureEntityType.Artist => "artists",
            PictureEntityType.Song => "songs",
            PictureEntityType.Album => "albums",
            _ => null
        };

        model.Filename = picture.Filename;
        model.ImageFile = !string.IsNullOrWhiteSpace(entityType)
            ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/images/{entityType}/uploads/{picture.EntityId}/{picture.Filename}.webp"
            : $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/images/no_image_1024.webp";

        model.Thumb64 = PreparePictureThumbUrl(picture.EntityId, picture.Filename, 64);
        model.Thumb128 = PreparePictureThumbUrl(picture.EntityId, picture.Filename, 128);
        model.Thumb256 = PreparePictureThumbUrl(picture.EntityId, picture.Filename, 256);
        model.Thumb512 = PreparePictureThumbUrl(picture.EntityId, picture.Filename, 512);
        model.Thumb1024 = PreparePictureThumbUrl(picture.EntityId, picture.Filename, 1024);

        return await Task.FromResult(model);
    }
}