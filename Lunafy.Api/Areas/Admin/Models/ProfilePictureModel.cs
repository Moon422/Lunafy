using Lunafy.Api.Models;

namespace Lunafy.Api.Areas.Admin.Models;

public record PictureModel : BaseEntityModel
{
    public int PictureEntityTypeId { get; set; }
    public string PictureEntityTypeIdStr { get; set; }
    public int EntityId { get; set; }
    public string? Filename { get; set; }
    public string? ImageFile { get; set; }
    public string Thumb1024 { get; set; }
    public string Thumb512 { get; set; }
    public string Thumb256 { get; set; }
    public string Thumb128 { get; set; }
    public string Thumb64 { get; set; }
}
