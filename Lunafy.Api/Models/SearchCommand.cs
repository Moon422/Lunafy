using System;

namespace Lunafy.Api.Models;

public abstract class SearchCommand
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int PageIndex => Math.Clamp(PageNumber, 1, int.MaxValue) - 1;
}