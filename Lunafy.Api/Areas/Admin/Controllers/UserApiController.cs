using System;
using System.Threading.Tasks;
using AutoMapper;
using Lunafy.Api.Areas.Admin.Factories;
using Lunafy.Api.Areas.Admin.Models.Users;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;
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
    private readonly IMapper _mapper;

    public UserApiController(IUserService userService,
        IUserModelsFactory userModelsFactory,
        IWorkContext workContext,
        IMapper mapper)
    {
        _workContext = workContext;
        _userModelsFactory = userModelsFactory;
        _userService = userService;
        _mapper = mapper;
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

        var model = await _userModelsFactory.PrepareUserReadModelAsync(_mapper.Map<UserReadModel>(userEntity), userEntity);
        response.Data = model;
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserCreateModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var userEntity = _mapper.Map<User>(model);
        userEntity.RequirePasswordReset = true;
        await _userService.CreateUserAsync(userEntity);

        var userModel = await _userModelsFactory.PrepareUserReadModelAsync(_mapper.Map<UserReadModel>(userEntity), userEntity);
        var response = new HttpResponseModel<UserReadModel>
        {
            Data = userModel
        };

        return CreatedAtAction(nameof(Get), new { id = userEntity.Id }, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UserCreateModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var userEntity = _mapper.Map<User>(model);
        await _userService.UpdateUserAsync(userEntity);

        var userModel = await _userModelsFactory.PrepareUserReadModelAsync(_mapper.Map<UserReadModel>(userEntity), userEntity);
        var response = new HttpResponseModel<UserReadModel>
        {
            Data = userModel
        };

        return Ok(response);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(AuthModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var response = new HttpResponseModel();
        if (model.UserId <= 0 || (await _userService.GetUserByIdAsync(model.UserId)) is null)
        {
            response.Errors.Add("Invalid user selected. Please try again.");
            return BadRequest(response);
        }

        await _userService.CreatePasswordAsync(model.UserId, model.Password);
        response.Message = "Password updated.";

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        User? userEntity;
        var response = new HttpResponseModel();
        if (id <= 0 || (userEntity = await _userService.GetUserByIdAsync(id)) is null)
        {
            response.Errors.Add("Invalid user selected. Please try again.");
            return BadRequest(response);
        }

        await _userService.DeleteUserAsync(userEntity);
        return NoContent();
    }
}