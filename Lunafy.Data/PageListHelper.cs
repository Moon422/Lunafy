using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Data;

public static class PagedListHelper
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageIndex = 0, int pageSize = int.MaxValue) where T : BaseEntity
    {
        var count = await query.CountAsync();
        return new PagedList<T>(await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(), count, pageIndex, pageSize);
    }
}