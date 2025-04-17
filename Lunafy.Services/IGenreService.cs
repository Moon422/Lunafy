using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface IGenreService
{
    Task CreateGenreAsync(Genre genre);
    Task<Genre?> GetGenreByIdAsync(int id);
    Task<IList<Genre>> GetAllGenresAsync(bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Genre>> GetAllGenresAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<Genre>> FindGenresAsync(FindGenresCommand findCommand);
    Task UpdateGenreAsync(Genre genre);
    Task DeleteGenreAsync(Genre genre);
}
