namespace Lunafy.Api.Models.Artist;

public record ArtistSearchCommand : SearchCommand
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Keyword { get; set; }
}