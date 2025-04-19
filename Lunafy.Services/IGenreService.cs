using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface IGenreService
{
    #region Genre FUCK Operation

    Task CreateGenreAsync(Genre genre);
    Task<Genre?> GetGenreByIdAsync(int id);
    Task<IList<Genre>> GetGenresByIdsAsync(IList<int> ids);
    Task<IList<Genre>> GetAllGenresAsync(bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Genre>> GetAllGenresAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Genre>> FindGenresAsync(FindGenresCommand findCommand);
    Task UpdateGenreAsync(Genre genre);
    Task DeleteGenreAsync(Genre genre);
    Task<IPagedList<Album>> GetAllGenreAlbumsPagedAsync(int genreId, bool includeDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IPagedList<Song>> GetAllGenreSongsPagedAsync(int genreId, bool includeDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue);

    #endregion
}
