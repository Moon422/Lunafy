using System;
using System.Threading.Tasks;
using Lunafy.Api.Models;
using Lunafy.Api.Models.User;
using Lunafy.Core.Domains;
using Lunafy.Data;
using Lunafy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Lunafy.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserApiController : ControllerBase
{
    private readonly ITransactionManager _transactionManager;
    private readonly IUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public UserApiController(ITransactionManager transactionManager,
        IUserService userService,
        IRefreshTokenService refreshTokenService,
        IAuthService authService,
        IConfiguration configuration)
    {
        _transactionManager = transactionManager;
        _userService = userService;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
        _authService = authService;
    }

    #region Utilities

    private async Task<string> LoginAsync(User user)
    {
        var jwt = await _authService.GenerateJwtToken(user);
        await _authService.GenerateRefreshToken(user);

        user.LastLogin = DateTime.UtcNow;
        await _userService.UpdateUserAsync(user);

        return jwt;
    }

    #endregion

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userService.GetUserByUseranmeAsync(model.Username);
        var response = new HttpResponseModel<LoginResponseModel>();
        if (user is null || !await _userService.VerifyPasswordAsync(user.Id, model.Password))
        {
            response.Errors.Add("Invalid credentials.");
            return BadRequest(response);
        }

        var userModel = user.ToModel();
        var jwt = await LoginAsync(user);
        var loginResponse = new LoginResponseModel
        {
            User = userModel,
            Jwt = jwt
        };

        response.Data = loginResponse;

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegistrationModel model)
    {
        var user = (await _userService.GetUserByEmailAsync(model.Email))
            ?? await _userService.GetUserByUseranmeAsync(model.Username);

        if (user is not null)
        {
            return BadRequest("User with email or username already exists.");
        }

        user = model.ToEntity();
        try
        {
            await _transactionManager.ExecuteAsync(async () =>
            {
                await _userService.CreateUserAsync(user);
                await _userService.CreatePasswordAsync(user.Id, model.Password);
            });

            var userModel = user.ToModel();
            var jwt = await LoginAsync(user);
            var response = new LoginResponseModel
            {
                User = userModel,
                Jwt = jwt
            };

            return CreatedAtAction(nameof(Details), response);
        }
        catch
        {
            return Problem();
        }
    }

    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var token = HttpContext.Request.Cookies["refresh-token"];
        var response = new HttpResponseModel<LoginResponseModel>();
        if (string.IsNullOrWhiteSpace(token))
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            response.Errors.Add("Invalid authentication token.");

            return Unauthorized(response);
        }

        var refreshToken = await _refreshTokenService.GetRefreshTokenByTokenAsync(token);
        if (refreshToken is null)
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            response.Errors.Add("Invalid authentication token.");

            return Unauthorized(response);
        }

        if (!refreshToken.IsValid)
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            response.Errors.Add("Invalid authentication token.");

            return Unauthorized(response);
        }

        var user = await _userService.GetUserByIdAsync(refreshToken.UserId);
        if (user is null)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            response.Errors.Add("User not found.");

            return BadRequest(response);
        }

        var expirationDurationRemaining = refreshToken.ExpiryDate - DateTime.UtcNow;
        if (expirationDurationRemaining < TimeSpan.Zero)
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            response.Errors.Add("Invalid authentication token.");

            return Unauthorized(response);
        }

        var expirationHourRemaining = expirationDurationRemaining.TotalHours;
        if (expirationHourRemaining <= 24)
        {
            await _authService.GenerateRefreshToken(user);
        }

        var userModel = user.ToModel();
        var jwt = await _authService.GenerateJwtToken(user);
        var loginResponse = new LoginResponseModel
        {
            User = userModel,
            Jwt = jwt
        };

        response.StatusCode = StatusCodes.Status200OK;
        response.Data = loginResponse;

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Details()
    {
        return Ok();
    }
}
