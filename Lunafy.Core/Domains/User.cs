using System;

namespace Lunafy.Core.Domains;

public class User : BaseEntity, ICreationLogged, IModificationLogged, ISoftDeleted
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsArtist { get; set; }
    public bool IsInactive { get; set; }
    public DateTime? InactiveTill { get; set; }
    public int RequirePasswordReset { get; set; }
    public DateTime LastLogin { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
