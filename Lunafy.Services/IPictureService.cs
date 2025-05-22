using System;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface IPictureService
{
    Task<Picture?> GetPictureByIdAsync(int id);
    Task CreatePictureAsync(Picture picture);
    Task DeletePictureAsync(Picture picture);
    string? GetPictureDirectory(Picture picture, bool thumbDirectory = false);
    string? GetPicturePath(Picture picture, bool thumbDirectory = false);
    Task<IPagedList<Picture>> SearchPicturesAsync(int? pictureEntityTypeId = null, int? entityId = null, string filename = "", DateTime? createdOnFrom = null, DateTime? createdOnTill = null, int pageIndex = 0, int pageSize = int.MaxValue);
}
