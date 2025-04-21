using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public static class ArtistCacheDefaults
{
    public static string EntityTypeName => EntityCacheDefaults<Artist>.Prefix;
    public static CacheKey UserCanEditCacheKey => new($"Lunafy.{EntityTypeName}.UserCanEdit.{{0}}.{{1}}", UserCanEditPrefix);
    public static CacheKey UserCanDeleteCacheKey => new($"Lunafy.{EntityTypeName}.UserCanDelete.{{0}}.{{1}}", UserCanDeletePrefix);

    public static string Prefix => EntityCacheDefaults<Artist>.Prefix;
    public static string UserCanEditPrefix => $"Lunafy.{EntityTypeName}.UserCanEdit.";
    public static string UserCanDeletePrefix => $"Lunafy.{EntityTypeName}.UserCanDelete.";
}