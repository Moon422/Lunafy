using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public class ArtistService : IArtistService
{
    private readonly IRepository<Artist> _artistRepository;

    public ArtistService(IRepository<Artist> artistRepository)
    {
        _artistRepository = artistRepository ?? throw new ArgumentNullException(nameof(artistRepository));
    }

    public async Task CreateArtistAsync(Artist artist)
    {
        ArgumentNullException.ThrowIfNull(artist, nameof(artist));

        await _artistRepository.InsertAsync(artist);
    }

    public async Task<Artist?> GetArtistByIdAsync(int id, bool includeDeleted = false)
    {
        if (id <= 0)
            return null;

        return await _artistRepository.GetByIdAsync(id, (cache) => default, includeDeleted: includeDeleted);
    }

    public async Task<IList<Artist>> GetAllArtistsAsync(bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _artistRepository.GetAllAsync((cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<Artist>> GetAllArtistsAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _artistRepository.GetAllAsync(pageIndex, pageSize, (cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<Artist>> FindArtistsAsync(FindArtistsCommand findCommand, bool? deleted = false)
    {
        ArgumentNullException.ThrowIfNull(findCommand, nameof(findCommand));

        int pageIndex = findCommand.PageIndex >= 0 ? findCommand.PageIndex : 0;
        int pageSize = findCommand.PageSize > 0 ? findCommand.PageSize : 1;

        Func<IQueryable<Artist>, IQueryable<Artist>> queryFunc = query =>
        {
            if (deleted.HasValue)
                query = query.Where(a => a.Deleted != deleted);

            if (!string.IsNullOrWhiteSpace(findCommand.Firstname))
            {
                query = query.Where(a => a.Firstname.ToLower().Contains(findCommand.Firstname.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Lastname))
            {
                query = query.Where(a => a.Lastname.ToLower().Contains(findCommand.Lastname.ToLower()));
            }

            var keyword = findCommand.Keyword?.ToLower();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a =>
                    a.Firstname.ToLower().Contains(keyword) ||
                    a.Lastname.ToLower().Contains(keyword) ||
                    (!string.IsNullOrWhiteSpace(a.Biography) && a.Biography.ToLower().Contains(keyword)));
            }

            return query;
        };

        return await queryFunc(_artistRepository.Table).ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task UpdateArtistAsync(Artist artist)
    {
        ArgumentNullException.ThrowIfNull(artist, nameof(artist));

        await _artistRepository.UpdateAsync(artist);
    }

    public async Task DeleteArtistAsync(Artist artist)
    {
        ArgumentNullException.ThrowIfNull(artist, nameof(artist));

        await _artistRepository.DeleteAsync(artist);
    }
}