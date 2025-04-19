using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public static class RefreshTokenCacheDefaults
{
    public static string EntityTypeName => EntityCacheDefaults<RefreshToken>.EntityTypeName;

    public static CacheKey ByTokenCacheKey => new($"Lunafy.{EntityTypeName}.ByToken.{{0}}", ByTokenPrefix);

    public static string Prefix => EntityCacheDefaults<RefreshToken>.Prefix;

    public static string ByTokenPrefix => $"Lunafy.{EntityTypeName}.ByToken.";
}