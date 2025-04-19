using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public class ArtistCacheEventConsumer : CacheEventConsumer<Artist>
{
    public ArtistCacheEventConsumer(ICacheManager cacheManager)
        : base(cacheManager)
    { }

    protected override async Task ClearCacheAsync(Artist entity, EntityEventType entityEventType)
    {
        await base.ClearCacheAsync(entity, entityEventType);
    }
}
