using System;

namespace Lunafy.Core.Domains;

public class Album : BaseEntity, ICreationLogged, IModificationLogged, ISoftDeleted
{
    public string Name { get; set; }
    public string Year { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Comment { get; set; }
    public Guid MusicBrainzId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
