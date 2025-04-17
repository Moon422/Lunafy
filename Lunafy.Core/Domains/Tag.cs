using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public class Tag : BaseEntity
{
    [Required, MaxLength(64)]
    public string Name { get; set; }
}
