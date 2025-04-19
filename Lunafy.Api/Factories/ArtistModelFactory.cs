using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IArtistService _artistService;

    private readonly int[] IMAGE_DIMENSION = { 64, 128, 256, 512, 1024 };

    public ArtistModelFactory(IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IArtistService artistService)
    {
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _artistService = artistService;
    }

    private FindArtistsCommand ArtistSearchToFindCommand(ArtistSearchCommand command)
    {
        return new FindArtistsCommand
        {
            PageNumber = command.PageNumber,
            PageSize = command.PageSize
        };
    }

    private string PrepareProfileImageUrl(Artist artist, int width)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is null.");

        var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/images/artists/{artist.Id}/{width}.webp";
        return url;
    }

    public async Task<ArtistReadModel> PrepareArtistReadModelAsync(ArtistReadModel model, Artist artist)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        ArgumentNullException.ThrowIfNull(artist, nameof(artist));

        model.ProfileImage64 = PrepareProfileImageUrl(artist, 64);
        model.ProfileImage128 = PrepareProfileImageUrl(artist, 128);
        model.ProfileImage256 = PrepareProfileImageUrl(artist, 256);
        model.ProfileImage512 = PrepareProfileImageUrl(artist, 512);

        return model;
    }

    public async Task<SearchResultModel<ArtistReadModel>> PrepareArtistReadSearchResultAsync(ArtistSearchCommand command)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        var findArtistCommand = ArtistSearchToFindCommand(command);
        var artistsResult = await _artistService.FindArtistsAsync(findArtistCommand, false);

        var artistModels = new List<ArtistReadModel>();
        foreach (var artist in artistsResult)
        {
            artistModels.Add(await PrepareArtistReadModelAsync(_mapper.Map<ArtistReadModel>(artist), artist));
        }

        var searchResult = new SearchResultModel<ArtistReadModel>
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