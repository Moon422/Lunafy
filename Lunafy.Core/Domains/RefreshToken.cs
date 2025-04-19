using System;
using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = Guid.NewGuid().ToString("N");
    public int UserId { get; set; }
    public bool IsValid { get; set; } = true;

    [Required]
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);
}
