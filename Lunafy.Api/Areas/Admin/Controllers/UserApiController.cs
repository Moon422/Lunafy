using System;
using System.Threading.Tasks;
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

        var searchResult = await _userModelsFactory.PrepareUserSearchResultAsync(command);
        var response = new HttpResponseModel<SearchResultModel<UserModel>>
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

        var response = new HttpResponseModel<UserModel>();
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

        var model = await _userModelsFactory.PrepareUserModelAsync(userEntity.ToModel(), userEntity);
        response.Data = model;
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }
        var response = new HttpResponseModel<UserModel>();
        if (await _userService.GetUserByEmailAsync(model.Email) is not null)
        {
            response.Errors.Add("User with same email already exists.");
            return BadRequest(response);
        }

        if (await _userService.GetUserByUseranmeAsync(model.Username) is not null)
        {
            response.Errors.Add("User with same username already exists.");
            return BadRequest(response);
        }

        var userEntity = model.ToEntity();
        userEntity.RequirePasswordReset = true;
        await _userService.CreateUserAsync(userEntity);

        var userModel = await _userModelsFactory.PrepareUserModelAsync(userEntity.ToModel(), userEntity);
        response.Data = userModel;

        return CreatedAtAction(nameof(Get), new { id = userEntity.Id }, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UserModel model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        if (user is null || !user.IsAdmin)
        {
            return Forbid();
        }

        var response = new HttpResponseModel<UserModel>();
        if (model.Id <= 0)
        {
            response.Errors.Add("User Id is required.");
            return BadRequest(response);
        }

        User? entity;
        if ((entity = await _userService.GetUserByEmailAsync(model.Email)) is not null && entity.Id != model.Id)
        {
            response.Errors.Add("User with same email already exists.");
            return BadRequest(response);
        }

        if ((entity = await _userService.GetUserByEmailAsync(model.Username)) is not null && entity.Id != model.Id)
        {
            response.Errors.Add("User with same username already exists.");
            return BadRequest(response);
        }

        entity = await _userService.GetUserByIdAsync(model.Id);
        if (entity is null)
        {
            return BadRequest("User not found.");
        }

        entity = model.UpdateEntity(entity);
        await _userService.UpdateUserAsync(entity);

        model = await _userModelsFactory.PrepareUserModelAsync(entity.ToModel(), entity);
        response.Data = model;

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

    [AllowAnonymous]
    [HttpGet("email-availability")]
    public async Task<IActionResult> CheckEmailAvailability([FromQuery] string email, [FromQuery] int? userId = null)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        return Ok(user is null || (userId.HasValue && user.Id == userId));
    }

    [AllowAnonymous]
    [HttpGet("username-availability")]
    public async Task<IActionResult> CheckUsernameAvailability([FromQuery] string username, [FromQuery] int? userId = null)
    {
        var user = await _userService.GetUserByUseranmeAsync(username);
        return Ok(user is null || (userId.HasValue && user.Id == userId));
    }
}
