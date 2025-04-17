namespace Lunafy.Core.Domains;

public class Genre : BaseEntity
{
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
