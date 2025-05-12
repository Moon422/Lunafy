using System;
using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public enum PictureEntityType
{
    User = 10,
    Artist = 20,
    Song = 30,
    Album = 40
}

public class Picture : BaseEntity, ICreationLogged
{
    [Required]
    public int PictureEntityTypeId { get; set; }

    [Required]
    public int EntityId { get; set; }

    public string Filename { get; set; }

    public DateTime CreatedOn { get; set; }
}
