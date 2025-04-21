namespace Lunafy.Core.Domains;

public class SongEditAccess : BaseEntity
{
    public int SongId { get; set; }
    public int UserId { get; set; }
    public bool CanEdit { get; set; } = true;
    public bool CanDelete { get; set; } = true;
}