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
        if (entityEventType == EntityEventType.Delete)
        {
            await RemoveByPrefixAsync(SongCacheDefaults.SongArtistIdsPrefix);
            await RemoveByPrefixAsync(ArtistCacheDefaults.UserCanEditPrefix);
            await RemoveByPrefixAsync(ArtistCacheDefaults.UserCanDeletePrefix);
            await RemoveByPrefixAsync(ArtistCacheDefaults.ArtistByMusicBrainzIdPrefix);
            await RemoveByPrefixAsync(ArtistCacheDefaults.ArtistProfilePicturePrefix);
        }

        if (entityEventType == EntityEventType.Update)
        {
            await RemoveByPrefixAsync(ArtistCacheDefaults.ArtistProfilePicturePrefix);
        }

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
