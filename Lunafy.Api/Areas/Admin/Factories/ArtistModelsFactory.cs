using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models;
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
    private readonly IPictureService _pictureService;
    private readonly IPictureModelFactory _pictureModelFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ArtistModelsFactory(IArtistService artistService,
        IPictureService pictureService,
        IPictureModelFactory pictureModelFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _artistService = artistService;
        _httpContextAccessor = httpContextAccessor;
        _pictureService = pictureService;
        _pictureModelFactory = pictureModelFactory;
    }

    public async Task<ArtistModel> PrepareArtistModelAsync(ArtistModel model, Artist artist)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        var picture = await _pictureService.GetPictureByIdAsync(artist.ProfilePictureId.GetValueOrDefault());

        model.ProfilePicture = picture is not null ? picture.ToModel()
            : new PictureModel
            {
                PictureEntityTypeId = (int)PictureEntityType.Artist,
                PictureEntityTypeIdStr = PictureEntityType.Artist.ToString(),
                EntityId = artist.Id
            };

        model.ProfilePicture = await _pictureModelFactory.PreparePictureModelAsync(model.ProfilePicture, picture);

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

    public async Task<SearchResultModel<PictureModel>> PrepareUploadedImagesAsync(int artistId, int page, int pageSize)
    {
        var images = await _pictureService.SearchPicturesAsync(pictureEntityTypeId: (int)PictureEntityType.Artist, entityId: artistId, pageIndex: page - 1, pageSize: pageSize);

        var pictureModels = new List<PictureModel>();
        foreach (var image in images)
        {
            pictureModels.Add(await _pictureModelFactory.PreparePictureModelAsync(image.ToModel(), image));
        }

        return new SearchResultModel<PictureModel>
        {
            Data = pictureModels,
            PageNumber = images.PageNumber,
            PageSize = images.PageSize,
            TotalItems = images.TotalItems,
            TotalPages = images.TotalPages
        };
    }
}