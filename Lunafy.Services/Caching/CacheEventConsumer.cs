using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;
using Lunafy.Data.Caching;
using Lunafy.Services.Events;

namespace Lunafy.Services.Caching;

public abstract partial class CacheEventConsumer<TEntity> :
    IConsumer<EntityInsertedEvent<TEntity>>,
    IConsumer<EntityUpdatedEvent<TEntity>>,
    IConsumer<EntityDeletedEvent<TEntity>>
    where TEntity : BaseEntity
{
    #region Fields

    protected readonly ICacheManager _cacheManager;

    #endregion

    #region Ctor

    protected CacheEventConsumer(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    #endregion

    #region Utilities

    protected virtual async Task ClearCacheAsync(TEntity entity, EntityEventType entityEventType)
    {
        await RemoveByPrefixAsync(EntityCacheDefaults<TEntity>.ByIdsPrefix);
        await RemoveByPrefixAsync(EntityCacheDefaults<TEntity>.AllPrefix);

        if (entityEventType != EntityEventType.Insert)
        {
            var key = _cacheManager.PrepareCacheKey(EntityCacheDefaults<TEntity>.ByIdCacheKey, entity);
            Remove(key);
        }

        await ClearCacheAsync(entity);
    }

    protected virtual Task ClearCacheAsync(TEntity entity)
    {
        return Task.CompletedTask;
    }

    protected virtual async Task RemoveByPrefixAsync(string prefix)
    {
        await _cacheManager.RemoveCacheByPrefixAsync(prefix);
    }

    public void Remove(CacheKey cacheKey)
    {
        _cacheManager.RemoveCacheByKey(cacheKey);
    }

    #endregion

    #region Methods

    public virtual async Task HandleEventAsync(EntityInsertedEvent<TEntity> eventMessage)
    {
        await ClearCacheAsync(eventMessage.Entity, EntityEventType.Insert);
    }

    public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
    {
        await ClearCacheAsync(eventMessage.Entity, EntityEventType.Update);
    }

    public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
    {
        await ClearCacheAsync(eventMessage.Entity, EntityEventType.Delete);
    }

    #endregion

    #region Nested

    protected enum EntityEventType
    {
        Insert,
        Update,
        Delete
    }

    #endregion
}