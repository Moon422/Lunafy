using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Data.Caching;

public interface ICacheManager
{
    CacheKey PrepareCacheKey(CacheKey key, params object[] parameters);
    Task<T?> GetAsync<T>(CacheKey key, Func<Task<T?>> fetch);
    Task<IList<T>> GetAsync<T>(CacheKey key, Func<Task<IList<T>>> fetch);
    Task<IPagedList<T>> GetAsync<T>(CacheKey key, Func<Task<IPagedList<T>>> fetch);
    Task ClearCacheAsync();
    void RemoveCacheByKey(CacheKey key);
    Task RemoveCacheByPrefixAsync(string prefix);
}