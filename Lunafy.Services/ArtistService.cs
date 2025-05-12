using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;
using Lunafy.Data.Caching;
using Lunafy.Services.Caching;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Services;

[ScopeDependency(typeof(IArtistService))]
public class ArtistService : IArtistService
{
    #region Fields

    private readonly IRepository<Artist> _artistRepository;
    private readonly IRepository<ArtistSongMapping> _artistSongMappingRepository;
    private readonly IRepository<Song> _songRepository;
    private readonly IRepository<Album> _albumRepository;
    private readonly IRepository<ArtistEditAccess> _artistEditAccessRepository;
    private readonly IRepository<Picture> _pictureRepository;
    private readonly ICacheManager _cacheManager;

    #endregion

    #region Constructor

    public ArtistService(IRepository<Artist> artistRepository,
        IRepository<ArtistSongMapping> artistSongMappingRepository,
        IRepository<Song> songRepository,
        IRepository<Album> albumRepository,
        IRepository<ArtistEditAccess> artistEditAccessRepository,
        IRepository<Picture> pictureRepository,
        ICacheManager cacheManager)
    {
        _artistRepository = artistRepository ?? throw new ArgumentNullException(nameof(artistRepository));
        _artistSongMappingRepository = artistSongMappingRepository;
        _songRepository = songRepository;
        _albumRepository = albumRepository;
        _artistEditAccessRepository = artistEditAccessRepository;
        _cacheManager = cacheManager;
        _pictureRepository = pictureRepository;
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

    public async Task<Artist?> GetArtistByMusicBrainzIdAsync(Guid guid, bool includeDeleted = false)
    {
        var cacheKey = _cacheManager.PrepareCacheKey(ArtistCacheDefaults.ArtistByMusicBrainzIdCacheKey, guid);
        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await _artistRepository.Table
            .Where(x => x.MusicBrainzId == guid)
            .FirstOrDefaultAsync();
        });
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
                query = query.Where(a => a.Deleted == deleted);

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

    public async Task<bool> CanBeEditedByUserAsync(int artistId, int userId)
    {
        if (artistId <= 0 || userId <= 0)
        {
            return false;
        }

        var cacheKey = _cacheManager.PrepareCacheKey(ArtistCacheDefaults.UserCanEditCacheKey, artistId, userId);
        return await _cacheManager.GetAsync(cacheKey, async () => await _artistEditAccessRepository.Table
            .AnyAsync(x => x.ArtistId == artistId && x.UserId == userId && x.CanEdit));
    }

    public async Task<bool> CanBeDeletedByUserAsync(int artistId, int userId)
    {
        if (artistId <= 0 || userId <= 0)
        {
            return false;
        }

        var cacheKey = _cacheManager.PrepareCacheKey(ArtistCacheDefaults.UserCanDeleteCacheKey, artistId, userId);
        return await _cacheManager.GetAsync(cacheKey, async () => await _artistEditAccessRepository.Table
            .AnyAsync(x => x.ArtistId == artistId && x.UserId == userId && x.CanDelete));
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
            return new PagedList<Song>([], 0, pageIndex, pageSize);

        var songQuery = _songRepository.Table;
        if (!includeDeleted)
            songQuery = songQuery.Where(s => !s.Deleted);

        return await _artistSongMappingRepository.Table
            .Where(x => x.ArtistId == artistId)
            .Join(songQuery, asm => asm.SongId, s => s.Id, (asm, s) => s)
            .ToPagedListAsync();
    }

    public async Task<IPagedList<Album>> GetAllArtistAlbumsPagedAsync(int artistId, bool includeDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        pageIndex = int.Clamp(pageIndex, 0, int.MaxValue);
        pageSize = int.Clamp(pageSize, 1, int.MaxValue);

        if (artistId <= 0)
            return new PagedList<Album>([], 0, pageIndex, pageSize);

        var songQuery = _songRepository.Table;
        if (!includeDeleted)
            songQuery = songQuery.Where(s => !s.Deleted);

        var albumQuery = _albumRepository.Table;
        if (!includeDeleted)
            albumQuery = albumQuery.Where(a => !a.Deleted);

        return await _artistSongMappingRepository.Table
            .Where(asm => asm.ArtistId == artistId)
            .Join(songQuery, asm => asm.SongId, s => s.Id, (asm, s) => s)
            .Join(albumQuery, s => s.AlbumId, a => a.Id, (s, a) => a)
            .ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<Picture?> GetProfilePictureAsync(int artistId)
    {
        if (artistId <= 0)
            return null;

        var cacheKey = _cacheManager.PrepareCacheKey(ArtistCacheDefaults.ArtistProfilePictureCacheKey, artistId);
        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await _pictureRepository.Table
                .Where(x => x.PictureEntityTypeId == (int)PictureEntityType.Artist
                    && x.EntityId == artistId)
                .FirstOrDefaultAsync();
        });
    }

    public async Task<IPagedList<Picture>> GetAllPicturesPagedAsync(FindArtistPicturesCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await _pictureRepository.Table
            .Where(x => x.PictureEntityTypeId == (int)PictureEntityType.Artist
                    && x.EntityId == command.ArtistId)
            .ToPagedListAsync(command.PageIndex, command.PageSize);
    }

    #endregion

    #endregion
}