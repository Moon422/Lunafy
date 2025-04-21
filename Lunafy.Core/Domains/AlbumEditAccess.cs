namespace Lunafy.Core.Domains;

public class AlbumEditAccess : BaseEntity
{
    public int AlbumId { get; set; }
    public int UserId { get; set; }
    public bool CanEdit { get; set; } = true;
    public bool CanDelete { get; set; } = true;
}
