using System;
using System.ComponentModel.DataAnnotations;

namespace Lunafy.Api.Models.Artist;

public record ArtistModel : BaseEntityModel
{
    [Required, MaxLength(128)]
    public string Firstname { get; set; }

    [Required, MaxLength(64)]
    public string Lastname { get; set; }

    [MaxLength(512)]
    public string? Biography { get; set; }

    public Guid? MusicBrainzId { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string ProfileImage512 { get; set; }
    public string ProfileImage256 { get; set; }
    public string ProfileImage128 { get; set; }
    public string ProfileImage64 { get; set; }
}
