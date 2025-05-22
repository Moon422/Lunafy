using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models;
using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;

namespace Lunafy.Api.Areas.Admin.Factories;

public interface IArtistModelsFactory
{
    Task<ArtistModel> PrepareArtistModelAsync(ArtistModel model, Artist artist);
    Task<SearchResultModel<ArtistModel>> PrepareArtistSearchResultAsync(ArtistSearchCommand searchCommand);
    Task<SearchResultModel<PictureModel>> PrepareUploadedImagesAsync(int artistId, int page, int pageSize);
}
