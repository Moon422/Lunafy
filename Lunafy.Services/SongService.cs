using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public class SongService : ISongService
{
    private readonly IRepository<Song> _songRepository;

    public SongService(IRepository<Song> songRepository)
    {
        _songRepository = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
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
}