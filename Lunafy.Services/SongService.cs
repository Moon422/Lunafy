using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;
using Lunafy.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Services;

[ScopeDependency(typeof(ISongService))]
public class SongService : ISongService
{
    private readonly IRepository<Song> _songRepository;
    private readonly IRepository<GenreSongMapping> _genreSongMappingRepository;
    private readonly IRepository<ArtistSongMapping> _artistSongMappingRepository;
    private readonly IGenreService _genreService;
    private readonly IArtistService _artistService;

    public SongService(IRepository<Song> songRepository,
        IRepository<GenreSongMapping> genreSongMappingRepository,
        IRepository<ArtistSongMapping> artistSongMappingRepository,
        IGenreService genreService,
        IArtistService artistService)
    {
        _songRepository = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
        _genreSongMappingRepository = genreSongMappingRepository;
        _artistSongMappingRepository = artistSongMappingRepository;
        _genreService = genreService;
        _artistService = artistService;
    }

    public async Task CreateSongAsync(Song song)
    {
        ArgumentNullException.ThrowIfNull(song, nameof(song));

        await _songRepository.InsertAsync(song);
    }

    public async Task<Song?> GetSongByIdAsync(int id, bool includeDeleted = false)
    {
        if (id <= 0)
            return null;

        return await _songRepository.GetByIdAsync(id, (cache) => default, includeDeleted: includeDeleted);
    }

    public async Task<IList<Song>> GetAllSongsAsync(bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _songRepository.GetAllAsync((cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<Song>> GetAllSongsAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _songRepository.GetAllAsync(pageIndex, pageSize, (cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<Song>> FindSongsAsync(FindSongsCommand findCommand, bool? deleted = false)
    {
        ArgumentNullException.ThrowIfNull(findCommand, nameof(findCommand));

        int pageIndex = findCommand.PageIndex >= 0 ? findCommand.PageIndex : 0;
        int pageSize = findCommand.PageSize > 0 ? findCommand.PageSize : 1;

        Func<IQueryable<Song>, IQueryable<Song>> queryFunc = query =>
        {
            if (deleted.HasValue)
                query = query.Where(s => s.Deleted != deleted);

            if (!string.IsNullOrWhiteSpace(findCommand.Title))
            {
                query = query.Where(s => s.Title.ToLower().Contains(findCommand.Title.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Year))
            {
                query = query.Where(s => s.Year == findCommand.Year);
            }

            if (findCommand.Years.Any())
            {
                query = query.Where(s => !string.IsNullOrWhiteSpace(s.Year) && findCommand.Years.Contains(s.Year));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(findCommand.YearFrom))
                {
                    query = query.Where(s => !string.IsNullOrWhiteSpace(s.Year) && s.Year.CompareTo(findCommand.YearFrom) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(findCommand.YearTo))
                {
                    query = query.Where(s => !string.IsNullOrWhiteSpace(s.Year) && s.Year.CompareTo(findCommand.YearTo) <= 0);
                }
            }

            if (findCommand.ReleaseDateFrom.HasValue)
            {
                query = query.Where(s => s.ReleaseDate.HasValue && s.ReleaseDate >= findCommand.ReleaseDateFrom.Value);
            }

            if (findCommand.ReleaseDateTo.HasValue)
            {
                query = query.Where(s => s.ReleaseDate.HasValue && s.ReleaseDate <= findCommand.ReleaseDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Keyword))
            {
                string keyword = findCommand.Keyword.ToLower();
                query = query.Where(s =>
                    s.Title.ToLower().Contains(keyword) ||
                    (s.Comment != null && s.Comment.ToLower().Contains(keyword)) ||
                    (s.Tags != null && s.Tags.ToLower().Contains(keyword)));
            }

            return query;
        };

        return await queryFunc(_songRepository.Table).ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task UpdateSongAsync(Song song)
    {
        ArgumentNullException.ThrowIfNull(song, nameof(song));

        await _songRepository.UpdateAsync(song);
    }

    public async Task DeleteSongAsync(Song song)
    {
        ArgumentNullException.ThrowIfNull(song, nameof(song));

        await _songRepository.DeleteAsync(song);
    }

    public async Task AddSongGenreAsync(int songId, int genreId)
    {
        Song? song;
        if (songId <= 0 || (song = await GetSongByIdAsync(songId)) is null)
            throw new EntityNotFoundException(nameof(Song));

        Genre? genre;
        if (genreId <= 0 || (genre = await _genreService.GetGenreByIdAsync(genreId)) is null)
            throw new EntityNotFoundException(nameof(Genre));

        await _genreSongMappingRepository.InsertAsync(new GenreSongMapping
        {
            GenreId = genre.Id,
            SongId = song.Id
        });
    }

    public async Task RemoveSongGenreAsync(int songId, int genreId)
    {
        Song? song;
        if (songId <= 0 || (song = await GetSongByIdAsync(songId)) is null)
            throw new EntityNotFoundException(nameof(Song));

        Genre? genre;
        if (genreId <= 0 || (genre = await _genreService.GetGenreByIdAsync(genreId)) is null)
            throw new EntityNotFoundException(nameof(Genre));

        var genreSongMapping = await _genreSongMappingRepository.Table
            .FirstOrDefaultAsync(gsm => gsm.SongId == song.Id && gsm.GenreId == genre.Id);

        if (genreSongMapping is null)
            throw new EntityNotFoundException(nameof(GenreSongMapping));

        await _genreSongMappingRepository.DeleteAsync(genreSongMapping);
    }

    public async Task AddSongArtistAsync(int songId, int artistId)
    {
        Song? song;
        if (songId <= 0 || (song = await GetSongByIdAsync(songId)) is null)
            throw new EntityNotFoundException(nameof(Song));

        Artist? artist;
        if (artistId <= 0 || (artist = await _artistService.GetArtistByIdAsync(artistId)) is null)
            throw new EntityNotFoundException(nameof(Artist));

        await _artistSongMappingRepository.InsertAsync(new ArtistSongMapping
        {
            ArtistId = artist.Id,
            SongId = song.Id
        });
    }

    public async Task RemoveSongArtistAsync(int songId, int artistId)
    {
        Song? song;
        if (songId <= 0 || (song = await GetSongByIdAsync(songId)) is null)
            throw new EntityNotFoundException(nameof(Song));

        Artist? artist;
        if (artistId <= 0 || (artist = await _artistService.GetArtistByIdAsync(artistId)) is null)
            throw new EntityNotFoundException(nameof(Artist));

        var artistSongMapping = await _artistSongMappingRepository.Table
            .FirstOrDefaultAsync(asm => asm.SongId == song.Id && asm.ArtistId == artist.Id);

        if (artistSongMapping is null)
            throw new EntityNotFoundException(nameof(ArtistSongMapping));

        await _artistSongMappingRepository.DeleteAsync(artistSongMapping);
    }
}