using System;
using Lunafy.Api.Models;

namespace Lunafy.Api.Areas.Admin.Models.Users;

public record UserSearchCommand : SearchCommand
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Keyword { get; set; }
    public DateTime? CreatedOnFromUtc { get; set; }
    public DateTime? CreatedOnTillUtc { get; set; }
}