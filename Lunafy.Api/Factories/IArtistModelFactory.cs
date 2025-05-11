using System.Threading.Tasks;
using Lunafy.Api.Models;
using Lunafy.Api.Models.Artist;
using Lunafy.Core.Domains;

namespace Lunafy.Api.Factories;

public interface IArtistModelFactory
{
    Task<ArtistModel> PrepareArtistModelAsync(ArtistModel model, Artist artist);
    Task<SearchResultModel<ArtistModel>> PrepareArtistSearchResultAsync(ArtistSearchCommand command);
}
