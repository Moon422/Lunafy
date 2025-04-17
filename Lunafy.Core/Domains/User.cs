using System;
using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public class User : BaseEntity, ICreationLogged, IModificationLogged, ISoftDeleted
{
    [Required, MaxLength(128)]
    public string Firstname { get; set; }

    [Required, MaxLength(128)]
    public string Lastname { get; set; }

    [Required, MaxLength(128)]
    public string Username { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsArtist { get; set; }
    public bool IsInactive { get; set; }
    public DateTime? InactiveTill { get; set; }
    public bool RequirePasswordReset { get; set; }
    public DateTime LastLogin { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
