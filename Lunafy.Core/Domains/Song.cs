using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Lunafy.Core.Domains;

public class Song : BaseEntity, ICreationLogged, IModificationLogged, ISoftDeleted
{
    [Required, MaxLength(128)]
    public string Title { get; set; }

    [Required, MaxLength(128)]
    public string FilePath { get; set; }
    public int TrackNumber { get; set; }
    public string? Year { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public float Duration { get; set; }
    public int Bitrate { get; set; }
    public int SampleRate { get; set; }
    public int BitDepth { get; set; }
    public int Channels { get; set; }
    public int Bpm { get; set; }

    [MaxLength(128)]
    public string? Comment { get; set; }
    public bool Explicit { get; set; }
    public int? AlbumId { get; set; }
    public Guid MusicBrainzId { get; set; }

    [MaxLength(512)]
    public string? Tags { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}

public class ArtistSongMapping : BaseEntity
{
    public int ArtistId { get; set; }
    public int SongId { get; set; }
}
