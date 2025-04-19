using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;

namespace Lunafy.Services;

[ScopeDependency(typeof(IArtistService))]
public class ArtistService : IArtistService
{
    #region Fields

    private readonly IRepository<Artist> _artistRepository;
    private readonly IRepository<ArtistSongMapping> _artistSongMappingRepository;
    private readonly IRepository<Song> _songRepository;

    #endregion

    #region Constructor

    public ArtistService(IRepository<Artist> artistRepository,
        IRepository<ArtistSongMapping> artistSongMappingRepository,
        IRepository<Song> songRepository)
    {
        _artistRepository = artistRepository ?? throw new ArgumentNullException(nameof(artistRepository));
        _artistSongMappingRepository = artistSongMappingRepository;
        _songRepository = songRepository;
    }

    #endregion

    #region Methods

    #region Artist FUCK Operations

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

    public async Task<IList<Artist>> GetArtistsByIdsAsync(IList<int> ids, bool includeDeleted = false)
    {
        if (ids is null || !ids.Any())
            return [];

        return await _artistRepository.GetByIdsAsync(ids, cache => default, includeDeleted);
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

    public async Task<IPagedList<Song>> GetAllArtistSongsPagedAsync(int artistId, bool includeDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        pageIndex = int.Clamp(pageIndex, 0, int.MaxValue);
        pageSize = int.Clamp(pageSize, 1, int.MaxValue);

        if (artistId <= 0)
            return new PagedList<Song>([], pageIndex, pageSize);

        var songQuery = _songRepository.Table;
        if (!includeDeleted)
            songQuery = songQuery.Where(s => !s.Deleted);

        return await _artistSongMappingRepository.Table
            .Where(x => x.ArtistId == artistId)
            .Join(songQuery, asm => asm.SongId, s => s.Id, (asm, s) => s)
            .ToPagedListAsync();
    }

    #endregion

    #endregion
}