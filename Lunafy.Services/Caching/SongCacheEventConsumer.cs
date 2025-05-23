using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Services.Caching;

public class SongCacheEventConsumer : CacheEventConsumer<Song>
{
    public SongCacheEventConsumer(ICacheManager cacheManager)
        : base(cacheManager)
    { }

    protected override async Task ClearCacheAsync(Song entity, EntityEventType entityEventType)
    {
        if (entityEventType == EntityEventType.Delete)
        {
            await RemoveByPrefixAsync(SongCacheDefaults.SongGenreIdsPrefix);
            await RemoveByPrefixAsync(SongCacheDefaults.SongArtistIdsPrefix);
        }

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
