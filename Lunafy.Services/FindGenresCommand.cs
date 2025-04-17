namespace Lunafy.Services;

public record FindGenresCommand : FindEntitiesCommand
{
    public string? Name { get; set; }
    public string? Keyword { get; set; }
}