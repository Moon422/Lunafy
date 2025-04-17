using System;

namespace Lunafy.Core.Domains;

public class Artist : BaseEntity, ICreationLogged, IModificationLogged, ISoftDeleted
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Biography { get; set; }
    public Guid MusicBrainzId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
