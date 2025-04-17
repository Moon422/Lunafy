using System;
using System.Collections;
using System.Collections.Generic;

namespace Lunafy.Services;

public record FindAlbumsCommand : FindEntitiesCommand
{
    public string? Name { get; set; }
    public IList<string> Years { get; set; } = [];
    public string? YearFrom { get; set; }
    public string? YearTo { get; set; }
    public DateTime? ReleaseDateFrom { get; set; }
    public DateTime? ReleaseDateTo { get; set; }
    public string? Keyword { get; set; }
}