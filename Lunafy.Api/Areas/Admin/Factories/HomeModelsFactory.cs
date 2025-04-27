using System;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models.Home;
using Lunafy.Services;

namespace Lunafy.Api.Areas.Admin.Factories;

public interface IHomeModelsFactory
{
    Task<TotalUsersStatModel> PrepareTotalUsersStatModelAsync();
}

public class HomeModelsFactory : IHomeModelsFactory
{
    private readonly IUserService _userService;

    public HomeModelsFactory(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<TotalUsersStatModel> PrepareTotalUsersStatModelAsync()
    {
        var now = DateTime.UtcNow;
        var presentDataFrom = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var previousDataTill = presentDataFrom.Subtract(new TimeSpan(1));
        var previousDataFrom = presentDataFrom.AddMonths(-1);

        var findUserCommand = new FindUsersCommand
        {
            CreatedOnFromUtc = presentDataFrom,
            CreatedOnTillUtc = now
        };
        var presentUserCount = await _userService.CountUsersAsync(findUserCommand);

        findUserCommand = new FindUsersCommand
        {
            CreatedOnFromUtc = previousDataFrom,
            CreatedOnTillUtc = previousDataTill
        };
        var previousUserCount = await _userService.CountUsersAsync(findUserCommand);

        var model = new TotalUsersStatModel
        {
            PresentDataTill = now,
            PresentUserCount = presentUserCount,
            PreviousDataTill = previousDataTill,
            PreviousUserCount = previousUserCount,
        };

        if (presentUserCount <= 0)
        {
            model.InfiniteIncrement = true;
            model.DidUserIncremented = true;
            return model;
        }

        var didUserIncremented = presentUserCount > previousUserCount ? true : presentUserCount < previousUserCount ? false : (bool?)null;
        var changePercentage = (1f * presentUserCount - presentUserCount) / presentUserCount * 100;
        model.DidUserIncremented = didUserIncremented;
        model.ChangePercentage = changePercentage;

        return model;
    }
}