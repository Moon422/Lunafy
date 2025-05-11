using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Services;
using Microsoft.AspNetCore.Http;

namespace Lunafy.Api.Areas.Admin.Factories;

[ScopeDependency(typeof(IArtistModelsFactory))]
public class ArtistModelsFactory : IArtistModelsFactory
{
    private readonly IArtistService _artistService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ArtistModelsFactory(IArtistService artistService,
        IHttpContextAccessor httpContextAccessor)
    {
        _artistService = artistService;
        _httpContextAccessor = httpContextAccessor;
    }

    private string PrepareProfileImageUrl(int artistId, int width)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext cannot be null.");

        var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/images/artists/profile/{artistId}/{width}.webp";
        return url;
    }

    public ProfilePictureModel? PrepareProfilePictureModel(ProfilePictureModel? model, int artistId)
    {
        if (model is null || artistId <= 0)
            return null;

        model.ProfileImage64 = PrepareProfileImageUrl(artistId, 64);
        model.ProfileImage128 = PrepareProfileImageUrl(artistId, 128);
        model.ProfileImage256 = PrepareProfileImageUrl(artistId, 256);
        model.ProfileImage512 = PrepareProfileImageUrl(artistId, 512);
        model.ProfileImage1024 = PrepareProfileImageUrl(artistId, 1024);

        return model;
    }

    public async Task<ArtistModel> PrepareArtistModelAsync(ArtistModel model, Artist artist)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        model.ProfilePicture = new ProfilePictureModel();
        model.ProfilePicture = PrepareProfilePictureModel(model.ProfilePicture, artist.Id);

        return await Task.FromResult(model);
    }

    public async Task<SearchResultModel<ArtistModel>> PrepareArtistSearchResultAsync(ArtistSearchCommand searchCommand)
    {
        ArgumentNullException.ThrowIfNull(searchCommand, nameof(searchCommand));

        var findCommand = searchCommand.ToFindCommand();
        var artistsResult = await _artistService.FindArtistsAsync(findCommand);

        var artistModels = new List<ArtistModel>();
        foreach (var artist in artistsResult)
        {
            artistModels.Add(await PrepareArtistModelAsync(artist.ToModel(), artist));
        }

        return new SearchResultModel<ArtistModel>
        {
            Data = artistModels,
            PageNumber = artistsResult.PageNumber,
            PageSize = artistsResult.PageSize,
            TotalItems = artistsResult.TotalItems,
            TotalPages = artistsResult.TotalPages
        };
    }
}