using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface ISongService
{
    #region Song FUCK Operations

    Task CreateSongAsync(Song song);
    Task<Song?> GetSongByIdAsync(int id, bool includeDeleted = false);
    Task<IList<Song>> GetAllSongsAsync(bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Song>> GetAllSongsAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Song>> FindSongsAsync(FindSongsCommand findCommand, bool? deleted = false);
    Task UpdateSongAsync(Song song);
    Task DeleteSongAsync(Song song);

    #endregion

    #region Genre FUCK Operations

    Task AddSongGenreAsync(int songId, int genreId);
    Task RemoveSongGenreAsync(int songId, int genreId);
    Task<IList<Genre>> GetAllSongGenresAsync(int songId);
    Task<IPagedList<Genre>> GetAllSongGenresPagedAsync(int songId, int pageIndex = 0, int pageSize = int.MaxValue);

    #endregion

    #region Artist FUCK Operations

    Task AddSongArtistAsync(int songId, int artistId);
    Task RemoveSongArtistAsync(int songId, int artistId);
    Task<IList<Artist>> GetAllSongArtistsAsync(int songId, bool includeDeleted = false);
    Task<IPagedList<Artist>> GetAllSongArtistsPagedAsync(int songId, bool includeDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue);

    #endregion
}