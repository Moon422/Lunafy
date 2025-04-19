using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public static class AlbumCacheDefaults
{
    public static string EntityTypeName => typeof(Album).Name.ToLowerInvariant();

    public static CacheKey AlbumGenreIdsCacheKey => new($"Lunafy.{EntityTypeName}.AlbumGenreIds.{{0}}", AlbumGenreIdsPrefix);

    public static string Prefix => EntityCacheDefaults<Album>.Prefix;

    public static string AlbumGenreIdsPrefix => $"Lunafy.{EntityTypeName}.AlbumGenreIds.";
}
