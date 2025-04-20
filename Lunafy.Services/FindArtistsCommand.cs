using System.Collections.Generic;

namespace Lunafy.Services;

public record FindArtistsCommand : FindEntitiesCommand
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Keyword { get; set; }
}