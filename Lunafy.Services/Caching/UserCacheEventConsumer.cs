using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public class UserCacheEventConsumer : CacheEventConsumer<User>
{
    public UserCacheEventConsumer(ICacheManager cacheManager)
        : base(cacheManager)
    { }

    protected override async Task ClearCacheAsync(User entity, EntityEventType entityEventType)
    {
        if (entityEventType == EntityEventType.Delete || entityEventType == EntityEventType.Update)
        {
            await RemoveByPrefixAsync(UserCacheDefaults.ByEmailPrefix);
            await RemoveByPrefixAsync(UserCacheDefaults.ByUsernamePrefix);
        }

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
