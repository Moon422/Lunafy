using System;
using System.ComponentModel.DataAnnotations;
using Lunafy.Api.Models;

namespace Lunafy.Api.Areas.Admin.Models.Artists;

public record ArtistModel : BaseEntityModel
{
    [Required, MaxLength(128)]
    public string Firstname { get; set; }

    [Required, MaxLength(64)]
    public string Lastname { get; set; }

    [MaxLength(512)]
    public string? Biography { get; set; }

    public Guid? MusicBrainzId { get; set; }

    public PictureModel? ProfilePicture { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
