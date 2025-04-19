using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public static class UserCacheDefaults
{
    public static string EntityTypeName => EntityCacheDefaults<Song>.EntityTypeName;

    public static CacheKey ByEmailUsernameCacheKey => new($"Lunafy.{EntityTypeName}.ByEmailUsername.{{0}}.{{1}}", ByEmailUsernamePrefix);

    public static string Prefix => EntityCacheDefaults<Song>.Prefix;

    public static string ByEmailUsernamePrefix => $"Lunafy.{EntityTypeName}.ByEmailUsername.";
}