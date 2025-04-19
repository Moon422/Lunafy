using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public static class UserCacheDefaults
{
    public static string EntityTypeName => EntityCacheDefaults<User>.EntityTypeName;

    public static CacheKey ByEmailCacheKey => new($"Lunafy.{EntityTypeName}.ByEmail.{{0}}", ByEmailPrefix);

    public static CacheKey ByUsernameCacheKey => new($"Lunafy.{EntityTypeName}.ByUsername.{{0}}", ByUsernamePrefix);

    public static string Prefix => EntityCacheDefaults<Song>.Prefix;

    public static string ByEmailPrefix => $"Lunafy.{EntityTypeName}.ByEmail.";

    public static string ByUsernamePrefix => $"Lunafy.{EntityTypeName}.ByUsername.";
}