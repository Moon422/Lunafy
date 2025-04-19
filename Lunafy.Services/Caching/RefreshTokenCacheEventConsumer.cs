using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public class RefreshTokenCacheEventConsumer : CacheEventConsumer<RefreshToken>
{
    public RefreshTokenCacheEventConsumer(ICacheManager cacheManager)
        : base(cacheManager)
    { }

    protected override async Task ClearCacheAsync(RefreshToken entity, EntityEventType entityEventType)
    {
        if (entityEventType != EntityEventType.Insert)
        {
            await RemoveByPrefixAsync(RefreshTokenCacheDefaults.ByTokenPrefix);
        }

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
