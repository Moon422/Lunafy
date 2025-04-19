namespace Lunafy.Api.Models;

public abstract record SearchCommand
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int PageIndex => int.Clamp(PageNumber, 1, int.MaxValue) - 1;
}