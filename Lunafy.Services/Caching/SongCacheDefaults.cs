using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public static class SongCacheDefaults
{
    public static string EntityTypeName => typeof(Song).Name.ToLowerInvariant();

    public static CacheKey SongGenreIdsCacheKey => new($"Lunafy.{EntityTypeName}.SongGenreIds.{{0}}", SongGenreIdsPrefix);

    public static CacheKey SongArtistIdsCacheKey => new($"Lunafy.{EntityTypeName}.SongArtistIds.{{0}}", SongArtistIdsPrefix);

    public static string Prefix => EntityCacheDefaults<Song>.Prefix;

    public static string SongGenreIdsPrefix => $"Lunafy.{EntityTypeName}.SongGenreIds.";

    public static string SongArtistIdsPrefix => $"Lunafy.{EntityTypeName}.SongArtistIds.";
}