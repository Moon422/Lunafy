using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public class Role : BaseEntity
{
    [Required, MaxLength(72)]
    public string Name { get; set; }
}

public class RoleUserMapping : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}