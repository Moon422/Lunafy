using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lunafy.Api.Models.User;
using Lunafy.Core.Domains;
using Lunafy.Data;
using Lunafy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lunafy.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserApiController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ITransactionManager _transactionManager;
    private readonly IUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;

    public UserApiController(IMapper mapper,
        ITransactionManager transactionManager,
        IUserService userService,
        IRefreshTokenService refreshTokenService,
        IConfiguration configuration)
    {
        _mapper = mapper;
        _transactionManager = transactionManager;
        _userService = userService;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
    }

    #region Utilities

    private async Task GenerateRefreshToken(User user)
    {
        var refreshToken = new RefreshToken
        {
            UserId = user.Id
        };

        await _refreshTokenService.CreateRefreshTokenAsync(refreshToken);

        HttpContext.Response.Cookies.Append(
            "refresh-token",
            refreshToken.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            }
        );
    }

    private async Task<string> LoginAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var roles = await _userService.GetUserRolesAsync(user.Id);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var secret = _configuration.GetSection("Secret").Value
            ?? throw new InvalidOperationException();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        await GenerateRefreshToken(user);
        user.LastLogin = DateTime.UtcNow;
        await _userService.UpdateUserAsync(user);

        return jwt;
    }

    #endregion

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userService.GetUserByUseranmeAsync(model.Username);
        if (user is null || !await _userService.VerifyPasswordAsync(user.Id, model.Password))
        {
            return BadRequest("Invalid credentials.");
        }

        var userModel = _mapper.Map<UserModel>(user);
        var jwt = await LoginAsync(user);
        var loginResponse = new LoginResponseModel
        {
            User = userModel,
            Jwt = jwt
        };

        return Ok(loginResponse);
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

        user = _mapper.Map<User>(model);
        try
        {
            await _transactionManager.ExecuteAsync(async () =>
            {
                await _userService.CreateUserAsync(user);
                await _userService.CreatePasswordAsync(user.Id, model.Password);
            });

            var userModel = _mapper.Map<UserModel>(user);
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

    [HttpGet("[action]")]
    public async Task<IActionResult> Details()
    {
        return Ok();
    }
}
