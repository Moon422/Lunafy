using Lunafy.Core.Domains;

namespace Lunafy.Data;

public interface IPagedList<T> where T : BaseEntity
{
    public int PageIndex { get; set; }
    public int TotalItems { get; }
    public int TotalPages { get; }
    public int PageSize { get; set; }

    public int PageNumber { get; }
    public bool HasPrevious { get; }
    public bool HasNext { get; }
}
