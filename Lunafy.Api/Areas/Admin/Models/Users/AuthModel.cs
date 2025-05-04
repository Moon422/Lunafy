using System.ComponentModel.DataAnnotations;
using Lunafy.Api.Models;

namespace Lunafy.Api.Areas.Admin.Models.Users;

public record AuthModel : BaseModel
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(10)]
    public string Password { get; set; }
}