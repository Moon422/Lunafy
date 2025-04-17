using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Data.Caching;

public interface ICacheManager
{
    CacheKey PrepareCacheKey(CacheKey key, params object[] parameters);
    Task<T?> GetAsync<T>(CacheKey key, Func<Task<T?>> fetch) where T : BaseEntity;
    Task<IList<T>> GetAsync<T>(CacheKey key, Func<Task<IList<T>>> fetch) where T : BaseEntity;
    void ClearCache();
    void RemoveCacheByKey(CacheKey key);
    void RemoveCacheByPrefix(string prefix);
}