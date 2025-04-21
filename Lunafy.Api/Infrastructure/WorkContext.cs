using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Services;
using Microsoft.AspNetCore.Http;

namespace Lunafy.Api.Infrastructure;

[ScopeDependency(typeof(IWorkContext))]
public class WorkContext : IWorkContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    private User? _cachedUser = null;

    public WorkContext(IHttpContextAccessor httpContextAccessor,
        IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_cachedUser is not null)
            return _cachedUser;

        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();

        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
        {
            return null;
        }

        var email = httpContext.User.FindFirstValue(ClaimTypes.Email);

        if (!string.IsNullOrWhiteSpace(email) && (_cachedUser = await _userService.GetUserByEmailAsync(email)) is not null)
        {
            return _cachedUser;
        }

        var username = httpContext.User.FindFirstValue(ClaimTypes.Name);

        if (!string.IsNullOrWhiteSpace(username) && (_cachedUser = await _userService.GetUserByUseranmeAsync(username)) is not null)
        {
            return _cachedUser;
        }

        return null;
    }
}