using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lunafy.Api.Service;

[ScopeDependency(typeof(IAuthService))]
public class AuthService : IAuthService
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public AuthService(IRefreshTokenService refreshTokenService,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _refreshTokenService = refreshTokenService;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _configuration = configuration;
    }

    public async Task<string> GenerateJwtToken(User user)
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

        if (_httpContextAccessor.HttpContext is not null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });
        }

        return jwt;
    }

    public async Task GenerateRefreshToken(User user)
    {
        var refreshToken = new RefreshToken
        {
            UserId = user.Id
        };

        await _refreshTokenService.CreateRefreshTokenAsync(refreshToken);

        if (_httpContextAccessor.HttpContext is not null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                "refresh-token",
                refreshToken.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                }
            );

            return;
        }

        throw new InvalidOperationException("Http context not found.");
    }
}
