using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Services;

public interface IPictureService
{
    Task<Picture?> GetPictureByIdAsync(int id);
    Task CreatePictureAsync(Picture picture);
    Task DeletePictureAsync(Picture picture);
    string? GetPictureDirectory(Picture picture);
}
