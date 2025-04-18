using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;

namespace Lunafy.Services;

[ScopeDependency(typeof(IGenreService))]
public class GenreService : IGenreService
{
    private readonly IRepository<Genre> _genreRepository;

    public GenreService(IRepository<Genre> genreRepository)
    {
        _genreRepository = genreRepository ?? throw new ArgumentNullException(nameof(genreRepository));
    }

    public async Task CreateGenreAsync(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre, nameof(genre));

        await _genreRepository.InsertAsync(genre);
    }

    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        if (id <= 0)
            return null;

        return await _genreRepository.GetByIdAsync(id, (cache) => default);
    }

    public async Task<IList<Genre>> GetAllGenresAsync(bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _genreRepository.GetAllAsync((cache) => default, includeDeleted, sortByIdDesc: sortByIdDesc);
    }

    public async Task<IPagedList<Genre>> GetAllGenresAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _genreRepository.GetAllAsync(pageIndex, pageSize, (cache) => default, includeDeleted, sortByIdDesc: sortByIdDesc);
    }

    public async Task<IPagedList<Genre>> FindGenresAsync(FindGenresCommand findCommand)
    {
        ArgumentNullException.ThrowIfNull(findCommand, nameof(findCommand));

        int pageIndex = findCommand.PageIndex >= 0 ? findCommand.PageIndex : 0;
        int pageSize = findCommand.PageSize > 0 ? findCommand.PageSize : 1;

        Func<IQueryable<Genre>, IQueryable<Genre>> queryFunc = query =>
        {
            if (!string.IsNullOrWhiteSpace(findCommand.Name))
            {
                query = query.Where(g => g.Name.ToLower().Contains(findCommand.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Keyword))
            {
                query = query.Where(g => g.Name.ToLower().Contains(findCommand.Keyword.ToLower()));
            }

            return query;
        };

        return await queryFunc(_genreRepository.Table).ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task UpdateGenreAsync(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre, nameof(genre));

        await _genreRepository.UpdateAsync(genre);
    }

    public async Task DeleteGenreAsync(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre, nameof(genre));

        await _genreRepository.DeleteAsync(genre);
    }
}