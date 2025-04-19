using System;
using System.ComponentModel.DataAnnotations;

namespace Lunafy.Api.Models.Artist;

public abstract record BaseArtistModel : BaseEntityModel
{
    [Required, MaxLength(128)]
    public string Firstname { get; set; }

    [Required, MaxLength(64)]
    public string Lastname { get; set; }

    [MaxLength(512)]
    public string? Biography { get; set; }

    public Guid? MusicBrainzId { get; set; }
}
