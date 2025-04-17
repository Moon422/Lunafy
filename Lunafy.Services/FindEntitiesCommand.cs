namespace Lunafy.Services;

public abstract record FindEntitiesCommand
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int PageIndex => PageNumber - 1;
}
