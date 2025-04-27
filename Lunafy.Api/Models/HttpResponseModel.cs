using System.Collections.Generic;

namespace Lunafy.Api.Models;

public record HttpResponseModel
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IList<string> Errors { get; set; } = [];
}

public record HttpResponseModel<T> : HttpResponseModel
{
    public T? Data { get; set; }
}