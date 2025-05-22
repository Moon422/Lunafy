using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models;
using Lunafy.Core.Domains;

namespace Lunafy.Api.Areas.Admin.Factories;

public interface IPictureModelFactory
{
    Task<PictureModel> PreparePictureModelAsync(PictureModel model, Picture? picture);
}
