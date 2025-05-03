using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public UserModelsFactory(IUserService userService,
        IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<UserReadModel> PrepareUserReadModelAsync(UserReadModel model, User user)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        return await Task.FromResult(model);
    }

    public async Task<SearchResultModel<UserReadModel>> PrepareUserReadSearchResultAsync(UserSearchCommand searchCommand)
    {
        ArgumentNullException.ThrowIfNull(searchCommand, nameof(searchCommand));

        var findCommand = _mapper.Map<FindUsersCommand>(searchCommand);
        var usersResult = await _userService.FindUsersAsync(findCommand);

        var userModels = new List<UserReadModel>();
        foreach (var user in usersResult)
        {
            userModels.Add(await PrepareUserReadModelAsync(_mapper.Map<UserReadModel>(user), user));
        }

        return new SearchResultModel<UserReadModel>
        {
            Data = userModels,
            PageNumber = usersResult.PageNumber,
            PageSize = usersResult.PageSize,
            TotalItems = usersResult.TotalItems,
            TotalPages = usersResult.TotalPages
        };
    }
}