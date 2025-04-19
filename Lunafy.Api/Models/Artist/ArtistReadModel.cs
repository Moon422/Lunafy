using System;

namespace Lunafy.Api.Models.Artist;

public record ArtistReadModel : BaseArtistModel
{
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? ProfileImage512 { get; set; }
    public string? ProfileImage256 { get; set; }
    public string? ProfileImage128 { get; set; }
    public string? ProfileImage64 { get; set; }
}
