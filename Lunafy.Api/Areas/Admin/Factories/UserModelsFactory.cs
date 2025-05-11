using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models.Users;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Services;

namespace Lunafy.Api.Areas.Admin.Factories;

[ScopeDependency(typeof(IUserModelsFactory))]
public class UserModelsFactory : IUserModelsFactory
{
    private readonly IUserService _userService;

    public UserModelsFactory(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserModel> PrepareUserModelAsync(UserModel model, User user)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        return await Task.FromResult(model);
    }

    public async Task<SearchResultModel<UserModel>> PrepareUserSearchResultAsync(UserSearchCommand searchCommand)
    {
        ArgumentNullException.ThrowIfNull(searchCommand, nameof(searchCommand));

        var findCommand = searchCommand.ToFindCommand();
        var usersResult = await _userService.FindUsersAsync(findCommand);

        var userModels = new List<UserModel>();
        foreach (var user in usersResult)
        {
            userModels.Add(await PrepareUserModelAsync(user.ToModel(), user));
        }

        return new SearchResultModel<UserModel>
        {
            Data = userModels,
            PageNumber = usersResult.PageNumber,
            PageSize = usersResult.PageSize,
            TotalItems = usersResult.TotalItems,
            TotalPages = usersResult.TotalPages
        };
    }
}