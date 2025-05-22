using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;

namespace Lunafy.Services;

[ScopeDependency(typeof(IPictureService))]
public class PictureService : IPictureService
{
    private readonly IRepository<Picture> _pictureRepository;

    public PictureService(IRepository<Picture> pictureRepository)
    {
        _pictureRepository = pictureRepository;
    }

    public async Task<Picture?> GetPictureByIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _pictureRepository.GetByIdAsync(id, cache => default);
    }

    public async Task CreatePictureAsync(Picture picture)
    {
        ArgumentNullException.ThrowIfNull(picture, nameof(picture));

        await _pictureRepository.InsertAsync(picture);
    }

    public async Task DeletePictureAsync(Picture picture)
    {
        ArgumentNullException.ThrowIfNull(picture, nameof(picture));

        await _pictureRepository.DeleteAsync(picture);
    }

    public string? GetPictureDirectory(Picture picture, bool thumbDirectory = false)
    {
        if (picture is null)
        {
            return null;
        }

        var entityType = (PictureEntityType)picture.PictureEntityTypeId;
        if (!Enum.IsDefined(entityType))
        {
            return null;
        }

        var entityName = entityType switch
        {
            PictureEntityType.User => "users",
            PictureEntityType.Artist => "artists",
            PictureEntityType.Song => "songs",
            PictureEntityType.Album => "albums",
            _ => null
        };

        return Path.Join("images", entityName, !thumbDirectory ? "uploads" : "thumbs", picture.EntityId.ToString());
    }

    public string? GetPicturePath(Picture picture, bool thumbDirectory = false)
    {
        if (picture is null)
        {
            return null;
        }

        var entityType = (PictureEntityType)picture.PictureEntityTypeId;
        if (!Enum.IsDefined(entityType))
        {
            return null;
        }

        return Path.Join(GetPictureDirectory(picture, thumbDirectory), $"{picture.Filename}.webp");
    }

    public Task<IPagedList<Picture>> SearchPicturesAsync(int? pictureEntityTypeId = null, int? entityId = null, string filename = "", DateTime? createdOnFrom = null, DateTime? createdOnTill = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _pictureRepository.Table;

        if (pictureEntityTypeId.HasValue)
        {
            query = query.Where(x => x.PictureEntityTypeId == pictureEntityTypeId);
        }

        if (entityId.HasValue)
        {
            query = query.Where(x => x.EntityId == entityId);
        }

        if (!string.IsNullOrWhiteSpace(filename))
        {
            query = query.Where(x => x.Filename.ToLower() == filename.ToLower());
        }

        if (createdOnFrom.HasValue)
        {
            query = query.Where(x => x.CreatedOn >= createdOnFrom);
        }

        if (createdOnTill.HasValue)
        {
            query = query.Where(x => x.CreatedOn <= createdOnTill);
        }

        return query.ToPagedListAsync(pageIndex, pageSize);
    }
}