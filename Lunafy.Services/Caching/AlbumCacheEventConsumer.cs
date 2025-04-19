using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public class AlbumCacheEventConsumer : CacheEventConsumer<Album>
{
    public AlbumCacheEventConsumer(ICacheManager cacheManager)
        : base(cacheManager)
    { }

    protected override async Task ClearCacheAsync(Album entity, EntityEventType entityEventType)
    {
        await base.ClearCacheAsync(entity, entityEventType);
    }
}
