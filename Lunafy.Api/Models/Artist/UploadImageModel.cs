using Microsoft.AspNetCore.Http;

namespace Lunafy.Api.Models.Artist;

public record UploadImageModel : BaseModel
{
    public int ArtistId { get; set; }
    public IFormFileCollection Images { get; set; }
}