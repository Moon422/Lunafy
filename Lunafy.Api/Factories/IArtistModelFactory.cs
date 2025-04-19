using System.Threading.Tasks;
using Lunafy.Api.Models;
using Lunafy.Api.Models.Artist;
using Lunafy.Core.Domains;

namespace Lunafy.Api.Factories;

public interface IArtistModelFactory
{
    Task<ArtistReadModel> PrepareArtistReadModelAsync(ArtistReadModel model, Artist artist);
    Task<SearchResultModel<ArtistReadModel>> PrepareArtistReadSearchResultAsync(ArtistSearchCommand command);
}
