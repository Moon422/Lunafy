using System;
using Lunafy.Api.Models;

namespace Lunafy.Api.Areas.Admin.Models.Home;

public record TotalUsersStatModel : BaseModel
{
    public DateTime? PresentDataTill { get; set; }
    public int PresentUserCount { get; set; }

    public DateTime? PreviousDataTill { get; set; }
    public int PreviousUserCount { get; set; }

    public bool InfiniteIncrement { get; set; }
    public bool? DidUserIncremented { get; set; }
    public float ChangePercentage { get; set; }
}
