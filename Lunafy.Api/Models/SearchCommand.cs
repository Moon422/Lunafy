using System;

namespace Lunafy.Api.Models;

public abstract record SearchCommand
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = int.MaxValue;
    public int PageIndex => Math.Clamp(PageNumber, 1, int.MaxValue) - 1;
}