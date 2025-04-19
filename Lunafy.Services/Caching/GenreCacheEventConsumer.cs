using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public class GenreCacheEventConsumer : CacheEventConsumer<Genre>
{
    public GenreCacheEventConsumer(ICacheManager cacheManager)
        : base(cacheManager)
    { }

    protected override async Task ClearCacheAsync(Genre entity, EntityEventType entityEventType)
    {

        if (entityEventType == EntityEventType.Delete)
        {
            await RemoveByPrefixAsync(AlbumCacheDefaults.AlbumGenreIdsPrefix);
        }

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
