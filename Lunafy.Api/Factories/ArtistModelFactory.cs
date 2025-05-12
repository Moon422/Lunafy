using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Api.Models;
using Lunafy.Api.Models.Artist;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Services;
using Microsoft.AspNetCore.Http;

namespace Lunafy.Api.Factories;

[ScopeDependency(typeof(IArtistModelFactory))]
public class ArtistModelFactory : IArtistModelFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IArtistService _artistService;

    public ArtistModelFactory(IHttpContextAccessor httpContextAccessor,
        IArtistService artistService)
    {
        _httpContextAccessor = httpContextAccessor;
        _artistService = artistService;
    }

    private string PrepareProfileImageUrl(Artist artist, int width)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is null.");

        var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/images/artists/profile/{artist.Id}/{width}.webp";
        return url;
    }

    public async Task<ArtistModel> PrepareArtistModelAsync(ArtistModel model, Artist artist)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        ArgumentNullException.ThrowIfNull(artist, nameof(artist));

        model.ProfileImage64 = PrepareProfileImageUrl(artist, 64);
        model.ProfileImage128 = PrepareProfileImageUrl(artist, 128);
        model.ProfileImage256 = PrepareProfileImageUrl(artist, 256);
        model.ProfileImage512 = PrepareProfileImageUrl(artist, 512);

        return await Task.FromResult(model);
    }

    public async Task<SearchResultModel<ArtistModel>> PrepareArtistSearchResultAsync(ArtistSearchCommand command)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        var findArtistCommand = command.ToFindCommand();
        var artistsResult = await _artistService.FindArtistsAsync(findArtistCommand, false);

        var artistModels = new List<ArtistModel>();
        foreach (var artist in artistsResult)
        {
            artistModels.Add(await PrepareArtistModelAsync(artist.ToModel(), artist));
        }

        var searchResult = new SearchResultModel<ArtistModel>
        {
            Data = artistModels,
            PageNumber = artistsResult.PageNumber,
            PageSize = artistsResult.PageSize,
            TotalItems = artistsResult.TotalItems,
            TotalPages = artistsResult.TotalPages
        };

        return searchResult;
    }
}