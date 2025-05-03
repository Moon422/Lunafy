using System;
using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Factories;
using Lunafy.Api.Areas.Admin.Models.Users;
using Lunafy.Api.Models;
using Lunafy.Core.Infrastructure;
using Lunafy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize]
[Route("api/[area]/user")]
public class UserApiController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserModelsFactory _userModelsFactory;
    private readonly IWorkContext _workContext;

    public UserApiController(IUserService userService,
        IUserModelsFactory userModelsFactory,
        IWorkContext workContext)
    {
        _workContext = workContext;
        _userModelsFactory = userModelsFactory;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UserSearchCommand command)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        Math.Clamp(command.PageNumber, 1, int.MaxValue);
        Math.Clamp(command.PageSize, 1, int.MaxValue);

        var searchResult = await _userModelsFactory.PrepareUserReadSearchResultAsync(command);
        var response = new HttpResponseModel<SearchResultModel<UserReadModel>>
        {
            Data = searchResult
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var response = new HttpResponseModel<UserReadModel>();
        if (id <= 0)
        {
            response.Errors.Add("User Id cannot be less than zero. Provide a valid user Id.");
            return BadRequest(response);
        }

        var userEntity = await _userService.GetUserByIdAsync(id);
        if (userEntity is null)
        {
            response.Errors.Add($"User with Id {id} not found.");
            return NotFound(response);
        }


    }
}