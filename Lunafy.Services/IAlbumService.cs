using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface IAlbumService
{
    #region Album FUCK Operations

    Task CreateAlbumAsync(Album album);
    Task<Album?> GetAlbumByIdAsync(int id, bool includeDeleted = false);
    Task<IList<Album>> GetAllAlbumsAsync(bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Album>> GetAllAlbumsAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Album>> FindAlbumsAsync(FindAlbumsCommand findCommand, bool? deleted = false);
    Task UpdateAlbumAsync(Album album);
    Task DeleteAlbumAsync(Album album);
    Task<IPagedList<Artist>> GetAllAlbumArtistsPagedAsync(int albumId, bool includeDeleted, int pageIndex = 0, int pageSize = int.MaxValue);

    #endregion

    #region Album Genre Operations

    Task AddAlbumGenreAsync(int albumId, int genreId);
    Task RemoveAlbumGenreAsync(int albumId, int genreId);
    Task<IList<Genre>> GetAllAlbumGenresAsync(int albumId);
    Task<IPagedList<Genre>> GetAllAlbumGenresPagedAsync(int albumId, int pageIndex = 0, int pageSize = int.MaxValue);

    #endregion
}
