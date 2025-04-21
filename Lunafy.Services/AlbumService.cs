using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;
using Lunafy.Data.Caching;
using Lunafy.Services.Caching;
using Lunafy.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Services;

[ScopeDependency(typeof(IAlbumService))]
public class AlbumService : IAlbumService
{
    #region Fields

    private readonly IRepository<Album> _albumRepository;
    private readonly IRepository<GenreAlbumMapping> _genreAlbumMappingRepository;
    private readonly IRepository<Genre> _genreRepository;
    private readonly IRepository<Song> _songRepository;
    private readonly IRepository<ArtistSongMapping> _artistSongMappingRepository;
    private readonly IRepository<Artist> _artistRepository;
    private readonly IGenreService _genreService;
    private readonly ICacheManager _cacheManager;

    #endregion

    #region Constructor

    public AlbumService(IRepository<Album> albumRepository,
        IRepository<GenreAlbumMapping> genreAlbumMappingRepository,
        IRepository<Genre> genreRepository,
        IRepository<Song> songRepository,
        IRepository<ArtistSongMapping> artistSongMappingRepository,
        IRepository<Artist> artistRepository,
        IGenreService genreService,
        ICacheManager cacheManager)
    {
        _albumRepository = albumRepository;
        _genreAlbumMappingRepository = genreAlbumMappingRepository;
        _genreRepository = genreRepository;
        _songRepository = songRepository;
        _artistSongMappingRepository = artistSongMappingRepository;
        _artistRepository = artistRepository;
        _genreService = genreService;
        _cacheManager = cacheManager;
    }

    #endregion

    #region Methods

    #region Album FUCK Operations

    public async Task CreateAlbumAsync(Album album)
    {
        ArgumentNullException.ThrowIfNull(album, nameof(album));

        await _albumRepository.InsertAsync(album);
    }

    public async Task<Album?> GetAlbumByIdAsync(int id, bool includeDeleted = false)
    {
        if (id <= 0)
            return null;

        return await _albumRepository.GetByIdAsync(id, (cache) => default, includeDeleted: includeDeleted);
    }

    public async Task<IList<Album>> GetAllAlbumsAsync(bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _albumRepository.GetAllAsync((cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<Album>> GetAllAlbumsAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _albumRepository.GetAllAsync(pageIndex, pageSize, (cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<Album>> FindAlbumsAsync(FindAlbumsCommand findCommand, bool? deleted = false)
    {
        ArgumentNullException.ThrowIfNull(findCommand, nameof(findCommand));

        int pageIndex = findCommand.PageIndex >= 0 ? findCommand.PageIndex : 0;
        int pageSize = findCommand.PageSize > 0 ? findCommand.PageSize : 1;

        Func<IQueryable<Album>, IQueryable<Album>> queryFunc = query =>
        {
            var name = findCommand.Name?.ToLower();

            if (deleted.HasValue)
                query = query.Where(a => a.Deleted == deleted);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(a => a.Name.ToLower().Contains(name));
            }

            if (findCommand.Years.Any())
            {
                query = query.Where(a => !string.IsNullOrWhiteSpace(a.Year) && findCommand.Years.Contains(a.Year));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(findCommand.YearFrom))
                {
                    query = query.Where(a => !string.IsNullOrWhiteSpace(a.Year) && a.Year.CompareTo(findCommand.YearFrom) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(findCommand.YearTo))
                {
                    query = query.Where(a => !string.IsNullOrWhiteSpace(findCommand.YearFrom) && findCommand.YearFrom.CompareTo(a.Year) >= 0);
                }
            }

            if (findCommand.ReleaseDateFrom.HasValue)
            {
                query = query.Where(a => a.ReleaseDate.HasValue && a.ReleaseDate >= findCommand.ReleaseDateFrom.Value);
            }

            if (findCommand.ReleaseDateTo.HasValue)
            {
                query = query.Where(a => a.ReleaseDate.HasValue && a.ReleaseDate <= findCommand.ReleaseDateTo.Value);
            }

            var keyword = findCommand.Keyword?.ToLower();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a =>
                    a.Name.ToLower().Contains(keyword) ||
                    (!string.IsNullOrWhiteSpace(a.Comment) && a.Comment.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrWhiteSpace(a.Description) && a.Description.ToLower().Contains(keyword)));
            }

            return query;
        };

        return await queryFunc(_albumRepository.Table).ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task UpdateAlbumAsync(Album album)
    {
        ArgumentNullException.ThrowIfNull(album, nameof(album));

        await _albumRepository.UpdateAsync(album);
    }

    public async Task DeleteAlbumAsync(Album album)
    {
        ArgumentNullException.ThrowIfNull(album, nameof(album));

        await _albumRepository.DeleteAsync(album);
    }

    public async Task<IPagedList<Artist>> GetAllAlbumArtistsPagedAsync(int albumId, bool includeDeleted, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        pageIndex = int.Clamp(pageIndex, 0, int.MaxValue);
        pageSize = int.Clamp(pageSize, 1, int.MaxValue);

        if (albumId <= 0)
            return new PagedList<Artist>([], 0, pageIndex, pageSize);

        var songQuery = _songRepository.Table
            .Where(s => s.AlbumId == albumId);
        if (!includeDeleted)
            songQuery = songQuery.Where(s => !s.Deleted);

        var artistQuery = _artistRepository.Table;

        if (!includeDeleted)
            artistQuery = artistQuery.Where(a => !a.Deleted);

        return await songQuery
            .Join(_artistSongMappingRepository.Table, s => s.Id, asm => asm.SongId, (s, asm) => asm.ArtistId)
            .Join(artistQuery, aid => aid, a => a.Id, (aid, a) => a)
            .ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion

    #region Album Genre Operations

    public async Task AddAlbumGenreAsync(int albumId, int genreId)
    {
        Album? album;
        if (albumId <= 0 || (album = await GetAlbumByIdAsync(albumId)) is null)
            throw new EntityNotFoundException(nameof(Album));

        Genre? genre;
        if (genreId <= 0 || (genre = await _genreService.GetGenreByIdAsync(genreId)) is null)
            throw new EntityNotFoundException(nameof(Genre));

        await _genreAlbumMappingRepository.InsertAsync(new GenreAlbumMapping
        {
            GenreId = genre.Id,
            AlbumId = album.Id
        });
    }

    public async Task RemoveAlbumGenreAsync(int albumId, int genreId)
    {
        Album? album;
        if (albumId <= 0 || (album = await GetAlbumByIdAsync(albumId)) is null)
            throw new EntityNotFoundException(nameof(Album));

        Genre? genre;
        if (genreId <= 0 || (genre = await _genreService.GetGenreByIdAsync(genreId)) is null)
            throw new EntityNotFoundException(nameof(Genre));

        var genreAlbumMapping = await _genreAlbumMappingRepository.Table
            .FirstOrDefaultAsync(gam => gam.AlbumId == album.Id && gam.GenreId == genre.Id);

        if (genreAlbumMapping is null)
            throw new EntityNotFoundException(nameof(GenreAlbumMapping));

        await _genreAlbumMappingRepository.DeleteAsync(genreAlbumMapping);
    }

    public async Task<IList<Genre>> GetAllAlbumGenresAsync(int albumId)
    {
        if (albumId <= 0)
            return [];

        var albumGenreIdsCacheKey = _cacheManager.PrepareCacheKey(AlbumCacheDefaults.AlbumGenreIdsCacheKey,
            albumId);

        var genreIds = await _cacheManager.GetAsync<int>(albumGenreIdsCacheKey,
            async () => await _genreAlbumMappingRepository.Table
                .Where(x => x.AlbumId == albumId)
                .Select(x => x.GenreId)
                .ToListAsync());

        if (!genreIds.Any())
            return [];

        return await _genreService.GetGenresByIdsAsync(genreIds);
    }

    public async Task<IPagedList<Genre>> GetAllAlbumGenresPagedAsync(int albumId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        pageIndex = pageIndex >= 0 ? pageIndex : 0;
        pageSize = pageSize > 0 ? pageSize : 1;

        if (albumId <= 0)
            return new PagedList<Genre>([], 0, pageIndex, pageSize);

        return await _genreAlbumMappingRepository.Table
            .Where(gam => gam.AlbumId == albumId)
            .Join(_genreRepository.Table, gam => gam.GenreId, g => g.Id, (gam, g) => g)
            .ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion

    #endregion
}