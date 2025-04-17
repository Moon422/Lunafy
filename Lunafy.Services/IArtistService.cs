using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface IArtistService
{
    Task CreateArtistAsync(Artist artist);
    Task<Artist?> GetArtistByIdAsync(int id, bool includeDeleted = false);
    Task<IList<Artist>> GetAllArtistsAsync(bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Artist>> GetAllArtistsAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Artist>> FindArtistsAsync(FindArtistsCommand findCommand, bool? deleted = false);
    Task UpdateArtistAsync(Artist artist);
    Task DeleteArtistAsync(Artist artist);
}
