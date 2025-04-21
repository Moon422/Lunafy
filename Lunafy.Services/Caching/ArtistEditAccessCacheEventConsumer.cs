using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public class ArtistEditAccessCacheEventConsumer : CacheEventConsumer<ArtistEditAccess>
{
    public ArtistEditAccessCacheEventConsumer(ICacheManager cacheManager)
        : base(cacheManager)
    { }

    protected override async Task ClearCacheAsync(ArtistEditAccess entity, EntityEventType entityEventType)
    {
        await RemoveByPrefixAsync(ArtistCacheDefaults.UserCanEditPrefix);
        await RemoveByPrefixAsync(ArtistCacheDefaults.UserCanDeletePrefix);

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
