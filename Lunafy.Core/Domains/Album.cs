using System;
using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public class Album : BaseEntity, ICreationLogged, IModificationLogged, ISoftDeleted
{
    [Required, MaxLength(128), MinLength(1)]
    public string Name { get; set; }

    public string? Year { get; set; }

    public DateTime? ReleaseDate { get; set; }

    [MaxLength(256)]
    public string? Comment { get; set; }

    public Guid? MusicBrainzId { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
