using System;
using System.Collections.Generic;

namespace Lunafy.Services;

public record FindSongsCommand : FindEntitiesCommand
{
    public string? Title { get; set; }
    public string? Year { get; set; }
    public string? YearFrom { get; set; }
    public string? YearTo { get; set; }
    public List<string> Years { get; set; } = new List<string>();
    public DateTime? ReleaseDateFrom { get; set; }
    public DateTime? ReleaseDateTo { get; set; }
    public string? Keyword { get; set; }
}