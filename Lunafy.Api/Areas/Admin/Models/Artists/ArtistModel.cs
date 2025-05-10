using System;
using System.ComponentModel.DataAnnotations;
using Lunafy.Api.Models;

namespace Lunafy.Api.Areas.Admin.Models.Artists;

public abstract record ArtistModel : BaseEntityModel
{
    [Required, MaxLength(128)]
    public string Firstname { get; set; }

    [Required, MaxLength(64)]
    public string Lastname { get; set; }

    [MaxLength(512)]
    public string? Biography { get; set; }

    public Guid? MusicBrainzId { get; set; }
}

public record ArtistCreateModel : ArtistModel { }

public record ArtistReadModel : ArtistModel
{
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
