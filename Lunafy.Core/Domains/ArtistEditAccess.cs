namespace Lunafy.Core.Domains;

public class ArtistEditAccess : BaseEntity
{
    public int ArtistId { get; set; }
    public int UserId { get; set; }
    public bool CanEdit { get; set; } = true;
    public bool CanDelete { get; set; } = true;
}
