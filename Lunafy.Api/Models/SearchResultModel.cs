using System.Collections.Generic;
using System.Linq;
using Lunafy.Data;

namespace Lunafy.Api.Models;

public record SearchResultModel<T> : BaseModel
{
    public IList<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}