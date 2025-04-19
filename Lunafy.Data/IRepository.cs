using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data.Caching;

namespace Lunafy.Data;

public interface IRepository<T> where T : BaseEntity
{
    public IQueryable<T> Table { get; }

    Task<int> GetCountAsync();

    Task<IList<T>> GetAllAsync(Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false);

    Task<IList<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> func, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false);

    Task<IPagedList<T>> GetAllAsync(int pageIndex, int pageSize, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false);

    Task<IPagedList<T>> GetAllAsync(int pageIndex, int pageSize, Func<IQueryable<T>, IQueryable<T>> func, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false, bool sortByIdDesc = false);

    Task<IPagedList<T>> GetAllPagedAsync(int pageIndex = 0, int pageSize = int.MaxValue, Func<IQueryable<T>, IQueryable<T>>? func = null, bool includeDeleted = false, bool sortByIdDesc = false);

    Task<T?> GetByIdAsync(int id, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false);

    Task<IList<T>> GetByIdsAsync(IList<int> ids, Func<ICacheManager, CacheKey?>? getCacheKey = null, bool includeDeleted = false);

    Task InsertAsync(T entity, bool publishEvent = true);

    Task InsertAsync(IList<T> entities, bool publishEvent = true);

    Task UpdateAsync(T entity, bool publishEvent = true);

    Task UpdateAsync(IList<T> entities, bool publishEvent = true);

    Task DeleteAsync(T entity, bool publishEvent = true);

    Task DeleteAsync(IList<T> entities, bool publishEvent = true);

    Task<int> DeleteAsync(Expression<Func<T, bool>> predicate);

    Task TruncateAsync(bool resetIdentity = false);
}
