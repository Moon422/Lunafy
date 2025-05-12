using System;
using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public class Artist : BaseEntity, ICreationLogged, IModificationLogged, ISoftDeleted
{
    [Required, MaxLength(128)]
    public string Firstname { get; set; }

    [Required, MaxLength(64)]
    public string Lastname { get; set; }

    [MaxLength(512)]
    public string? Biography { get; set; }

    public int? ProfilePictureId { get; set; }

    public Guid? MusicBrainzId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
