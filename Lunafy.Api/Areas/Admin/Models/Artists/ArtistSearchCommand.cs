using Lunafy.Api.Models;

namespace Lunafy.Api.Areas.Admin.Models.Artists;

public record ArtistSearchCommand : SearchCommand
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Keyword { get; set; }
}