using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface ISongService
{
    Task CreateSongAsync(Song song);
    Task<Song?> GetSongByIdAsync(int id, bool includeDeleted = false);
    Task<IList<Song>> GetAllSongsAsync(bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Song>> GetAllSongsAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Song>> FindSongsAsync(FindSongsCommand findCommand, bool? deleted = false);
    Task UpdateSongAsync(Song song);
    Task DeleteSongAsync(Song song);
    Task AddSongGenreAsync(int songId, int genreId);
    Task RemoveSongGenreAsync(int songId, int genreId);
    Task AddSongArtistAsync(int songId, int artistId);
    Task RemoveSongArtistAsync(int songId, int artistId);
}