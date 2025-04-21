using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Events;
using Lunafy.Data.Caching;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Data;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    #region Fields

    private readonly LunafyDbContext _context;
    private readonly ICacheManager _cacheManager;
    private readonly IEventPublisher _eventPublisher;

    #endregion

    #region Properties

    public IQueryable<T> Table => _context.Set<T>();

    #endregion

    #region Constructors

    public Repository(LunafyDbContext context,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
    {
        _context = context;
        _cacheManager = cacheManager;
        _eventPublisher = eventPublisher;
    }

    #endregion

    protected virtual IQueryable<T> AddDeletedFilter(IQueryable<T> query, in bool includeDeleted)
    {
        if (includeDeleted)
            return query;

        if (typeof(T).GetInterface(nameof(ISoftDeleted)) == null)
            return query;

        return query.OfType<ISoftDeleted>().Where(entry => !entry.Deleted).OfType<T>();
    }

    public async Task<int> GetCountAsync()
    {
        return await Table.CountAsync();
    }

    public async Task<IList<T>> GetAllAsync(Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        var getAllAsync = async () =>
        {
            var query = AddDeletedFilter(Table, includeDeleted);

            if (sortByIdDesc)
            {
                query = query.OrderByDescending(e => e.Id);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            return await query.ToListAsync();
        };

        if (getCacheKey is null)
            return await getAllAsync();

        var cacheKey = getCacheKey(_cacheManager) ??
            _cacheManager.PrepareCacheKey(EntityCacheDefaults<T>.AllCacheKey);

        return await _cacheManager.GetAsync<T>(cacheKey, async () => await getAllAsync());
    }

    public async Task<IList<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> func, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        var getAllAsync = async () =>
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func(query);

            if (sortByIdDesc)
            {
                query = query.OrderByDescending(e => e.Id);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            return await query.ToListAsync();
        };

        if (getCacheKey is null)
            return await getAllAsync();

        var cacheKey = getCacheKey(_cacheManager) ??
            _cacheManager.PrepareCacheKey(EntityCacheDefaults<T>.AllCacheKey);

        return await _cacheManager.GetAsync<T>(cacheKey, async () => await getAllAsync());
    }

    public async Task<IPagedList<T>> GetAllAsync(int pageIndex, int pageSize, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        if (pageIndex < 0)
            pageIndex = 0;

        if (pageSize < 1)
            pageSize = 1;

        var getAllAsync = async () =>
        {
            var query = AddDeletedFilter(Table, includeDeleted);

            if (sortByIdDesc)
            {
                query = query.OrderByDescending(e => e.Id);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            return await query.ToPagedListAsync(pageIndex, pageSize);
        };

        if (getCacheKey is null)
            return await getAllAsync();

        var cacheKey = getCacheKey(_cacheManager) ??
            _cacheManager.PrepareCacheKey(EntityCacheDefaults<T>.AllCacheKey);

        var data = await _cacheManager.GetAsync<T>(cacheKey, async () => await getAllAsync());
        return data;
    }

    public async Task<IPagedList<T>> GetAllAsync(int pageIndex, int pageSize, Func<IQueryable<T>, IQueryable<T>> func, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        if (pageIndex < 0)
            pageIndex = 0;

        if (pageSize < 1)
            pageSize = 1;

        var getAllAsync = async () =>
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func(query);

            if (sortByIdDesc)
            {
                query = query.OrderByDescending(e => e.Id);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            return await query.ToPagedListAsync(pageIndex, pageSize);
        };

        if (getCacheKey is null)
            return await getAllAsync();

        var cacheKey = getCacheKey(_cacheManager) ??
            _cacheManager.PrepareCacheKey(EntityCacheDefaults<T>.AllCacheKey);

        var data = await _cacheManager.GetAsync(cacheKey, async () => await getAllAsync());
        return data;
    }

    public async Task<IPagedList<T>> GetAllPagedAsync(int pageIndex = 0, int pageSize = int.MaxValue, Func<IQueryable<T>, IQueryable<T>>? func = null, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        if (pageIndex < 0)
            pageIndex = 0;

        if (pageSize < 1)
            pageSize = 1;

        var query = AddDeletedFilter(Table, includeDeleted);
        if (func is not null)
            query = func(query);

        if (sortByIdDesc)
        {
            query = query.OrderByDescending(e => e.Id);
        }
        else
        {
            query = query.OrderBy(e => e.Id);
        }

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<T?> GetByIdAsync(int id, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false)
    {
        if (id <= 0)
            return null;

        var getByIdAsync = async () =>
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        };

        if (getCacheKey is null)
            return await getByIdAsync();

        var cacheKey = getCacheKey(_cacheManager) ??
            _cacheManager.PrepareCacheKey(EntityCacheDefaults<T>.ByIdCacheKey, id);

        return await _cacheManager.GetAsync(cacheKey, async () => await getByIdAsync());
    }

    public async Task<IList<T>> GetByIdsAsync(IList<int> ids, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false)
    {
        if (ids == null || !ids.Any())
            return new List<T>();

        var getByIdsAsync = async () =>
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = query.Where(x => ids.Contains(x.Id));

            return await query.ToListAsync();
        };

        if (getCacheKey is null)
            return await getByIdsAsync();

        var cacheKey = getCacheKey(_cacheManager) ??
            _cacheManager.PrepareCacheKey(EntityCacheDefaults<T>.ByIdsCacheKey, ids);

        return await _cacheManager.GetAsync<T>(cacheKey, async () => await getByIdsAsync());
    }

    public async Task InsertAsync(T entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        if (entity is ICreationLogged cEntity)
        {
            cEntity.CreatedOn = DateTime.UtcNow;
        }

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        if (publishEvent)
        {
            await _eventPublisher.EntityInsertedAsync(entity);
        }
    }

    public async Task InsertAsync(IList<T> entities, bool publishEvent = true)
    {
        if (entities == null || !entities.Any())
            return;

        foreach (var entity in entities)
        {
            if (entity is ICreationLogged cEntity)
            {
                cEntity.CreatedOn = DateTime.UtcNow;
            }
        }

        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        if (publishEvent)
        {
            foreach (var entity in entities)
            {
                await _eventPublisher.EntityInsertedAsync(entity);
            }
        }
    }

    public async Task UpdateAsync(T entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        if (entity is IModificationLogged mEntity)
        {
            mEntity.ModifiedOn = DateTime.UtcNow;
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();

        if (publishEvent)
        {
            await _eventPublisher.EntityUpdatedAsync(entity);
        }
    }

    public async Task UpdateAsync(IList<T> entities, bool publishEvent = true)
    {
        if (entities == null || !entities.Any())
            return;

        foreach (var entity in entities)
        {
            if (entity is IModificationLogged mEntity)
            {
                mEntity.ModifiedOn = DateTime.UtcNow;
            }
        }

        _context.UpdateRange(entities);
        await _context.SaveChangesAsync();

        if (publishEvent)
        {
            foreach (var entity in entities)
            {
                await _eventPublisher.EntityUpdatedAsync(entity);
            }
        }
    }

    public async Task DeleteAsync(T entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        if (entity is ISoftDeleted sdEntity)
        {
            sdEntity.Deleted = true;
            sdEntity.DeletedOn = DateTime.UtcNow;
            _context.Update(entity);
        }
        else
        {
            _context.Remove(entity);
        }

        await _context.SaveChangesAsync();

        if (publishEvent)
        {
            await _eventPublisher.EntityDeletedAsync(entity);
        }
    }

    public async Task DeleteAsync(IList<T> entities, bool publishEvent = true)
    {
        if (entities == null || !entities.Any())
            return;

        if (typeof(ISoftDeleted).IsAssignableFrom(typeof(T)))
        {
            foreach (var entity in entities)
            {
                (entity as ISoftDeleted)!.Deleted = true;
                (entity as ISoftDeleted)!.DeletedOn = DateTime.UtcNow;
            }

            _context.UpdateRange(entities);
        }
        else
        {
            _context.RemoveRange(entities);
        }

        await _context.SaveChangesAsync();

        if (publishEvent)
        {
            foreach (var entity in entities)
            {
                await _eventPublisher.EntityDeletedAsync(entity);
            }
        }
    }

    public async Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        IQueryable<T> query = Table.Where(predicate);

        if (typeof(ISoftDeleted).IsAssignableFrom(typeof(T)))
        {
            var entities = await query.ToListAsync();
            foreach (var entity in entities)
            {
                (entity as ISoftDeleted)!.Deleted = true;
                (entity as ISoftDeleted)!.DeletedOn = DateTime.UtcNow;
            }
            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();

            return entities.Count;
        }

        return await query.ExecuteDeleteAsync();
    }

    public async Task TruncateAsync(bool resetIdentity = false)
    {
        if (resetIdentity)
        {
            var tableName = _context.Set<T>().EntityType.GetTableName();
            if (!string.IsNullOrWhiteSpace(tableName))
                await _context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE @p0", tableName);
        }
        else
        {
            await Table.ExecuteDeleteAsync();
        }
    }
}