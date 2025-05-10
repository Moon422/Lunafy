using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Areas.Admin.Models.Users;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Services;

namespace Lunafy.Api.Areas.Admin.Factories;

public interface IArtistModelsFactory
{
    Task<SearchResultModel<ArtistReadModel>> PrepareArtistReadSearchResultAsync(ArtistSearchCommand searchCommand);
}

[ScopeDependency(typeof(IArtistModelsFactory))]
public class ArtistModelsFactory : IArtistModelsFactory
{
    private readonly IArtistService _artistService;
    private readonly IMapper _mapper;

    public ArtistModelsFactory(IArtistService artistService,
        IMapper mapper)
    {
        _mapper = mapper;
        _artistService = artistService;
    }

    public async Task<ArtistReadModel> PrepareArtistReadModelAsync(ArtistReadModel model, Artist artist)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        return await Task.FromResult(model);
    }

    public async Task<SearchResultModel<ArtistReadModel>> PrepareArtistReadSearchResultAsync(ArtistSearchCommand searchCommand)
    {
        ArgumentNullException.ThrowIfNull(searchCommand, nameof(searchCommand));

        var findCommand = _mapper.Map<FindArtistsCommand>(searchCommand);
        var artistsResult = await _artistService.FindArtistsAsync(findCommand);

        var artistModels = new List<ArtistReadModel>();
        foreach (var artist in artistsResult)
        {
            artistModels.Add(await PrepareArtistReadModelAsync(_mapper.Map<ArtistReadModel>(artist), artist));
        }

        return new SearchResultModel<ArtistReadModel>
        {
            Data = artistModels,
            PageNumber = artistsResult.PageNumber,
            PageSize = artistsResult.PageSize,
            TotalItems = artistsResult.TotalItems,
            TotalPages = artistsResult.TotalPages
        };
    }
}