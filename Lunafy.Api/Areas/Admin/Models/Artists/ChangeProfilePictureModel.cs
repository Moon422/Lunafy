using Lunafy.Api.Models;
using Microsoft.AspNetCore.Http;

namespace Lunafy.Api.Areas.Admin.Models.Artists;

public record ChangeProfilePictureModel : BaseModel
{
    public int? PictureId { get; set; }
    public IFormFile? Image { get; set; }
}
