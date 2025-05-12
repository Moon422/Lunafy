using System;

namespace Lunafy.Services;

public record FindUsersCommand : FindEntitiesCommand
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Keyword { get; set; }
    public DateTime? CreatedOnFromUtc { get; set; }
    public DateTime? CreatedOnTillUtc { get; set; }
}

public record FindArtistPicturesCommand : FindEntitiesCommand
{
    public int ArtistId { get; set; }
}