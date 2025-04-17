using System.ComponentModel.DataAnnotations;

namespace Lunafy.Core.Domains;

public class Genre : BaseEntity
{
    [Required, MaxLength(64)]
    public string Name { get; set; }
}

public class GenreAlbumMapping : BaseEntity
{
    public int GenreId { get; set; }
    public int AlbumId { get; set; }
}

public class GenreSongMapping : BaseEntity
{
    public int GenreId { get; set; }
    public int SongId { get; set; }
}
