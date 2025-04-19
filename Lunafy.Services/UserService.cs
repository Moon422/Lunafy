using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;
using Lunafy.Data.Caching;
using Lunafy.Services.Caching;
using Lunafy.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Services;

[ScopeDependency(typeof(IUserService))]
public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Auth> _authRepository;
    private readonly IRepository<RoleUserMapping> _roleUserMappingRepository;
    private readonly IRoleService _roleService;
    private readonly ICacheManager _cacheManager;

    public UserService(IRepository<User> userRepository,
        IRepository<Auth> authRepository,
        IRepository<RoleUserMapping> roleUserMapping,
        IRoleService roleService,
        ICacheManager cacheManager)
    {
        _userRepository = userRepository;
        _authRepository = authRepository;
        _roleUserMappingRepository = roleUserMapping;
        _roleService = roleService;
        _cacheManager = cacheManager;
    }

    public async Task CreateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        await _userRepository.InsertAsync(user);
    }

    public async Task CreatePasswordAsync(int userId, string password)
    {
        if (userId <= 0)
            throw new ArgumentException($"{nameof(userId)} cannot be less then or equal to 0");

        if (string.IsNullOrWhiteSpace(password) || password.Length > 72)
            throw new ArgumentException($"{nameof(password)} cannot be empty or greater than 72 characters");

        var user = (await GetUserByIdAsync(userId)) ??
            throw new EntityNotFoundException(nameof(User));

        if (user.Deleted)
            throw new EntityNotFoundException(nameof(User));

        var auth = await _authRepository.Table.FirstOrDefaultAsync(a => a.UserId == user.Id);
        if (auth is null)
        {
            auth = new Auth
            {
                UserId = user.Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _authRepository.InsertAsync(auth);
            return;
        }

        auth.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await _authRepository.UpdateAsync(auth);
    }

    public async Task<bool> VerifyPasswordAsync(int userId, string password)
    {
        if (userId <= 0)
            throw new ArgumentException($"{nameof(userId)} cannot be less then or equal to 0");

        if (string.IsNullOrWhiteSpace(password) || password.Length > 72)
            throw new ArgumentException($"{nameof(password)} cannot be empty or greater than 72 characters");

        var user = (await GetUserByIdAsync(userId)) ??
            throw new EntityNotFoundException(nameof(User));

        if (user.Deleted)
            throw new EntityNotFoundException(nameof(User));

        var auth = (await _authRepository.Table.FirstOrDefaultAsync(a => a.UserId == user.Id)) ??
            throw new EntityNotFoundException(nameof(Auth));

        return BCrypt.Net.BCrypt.Verify(password, auth.PasswordHash);
    }

    public async Task<User?> GetUserByIdAsync(int id, bool includeDeleted = false)
    {
        if (id <= 0)
            return null;

        return await _userRepository.GetByIdAsync(id, (cache) => default, includeDeleted: includeDeleted);
    }

    public async Task<IList<User>> GetAllUsersAsync(bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _userRepository.GetAllAsync((cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<User>> GetAllUsersAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false)
    {
        return await _userRepository.GetAllAsync(pageIndex, pageSize, (cache) => default, includeDeleted, sortByIdDesc);
    }

    public async Task<IPagedList<User>> FindUsersAsync(FindUsersCommand findCommand, bool? deleted = false)
    {
        ArgumentNullException.ThrowIfNull(findCommand, nameof(findCommand));

        int pageIndex = findCommand.PageIndex >= 0 ? findCommand.PageIndex : 0;
        int pageSize = findCommand.PageSize > 0 ? findCommand.PageSize : 1;

        Func<IQueryable<User>, IQueryable<User>> queryFunc = query =>
        {
            if (deleted.HasValue)
                query = query.Where(u => u.Deleted != deleted);

            if (!string.IsNullOrWhiteSpace(findCommand.Firstname))
            {
                query = query.Where(u => u.Firstname.ToLower().Contains(findCommand.Firstname.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Lastname))
            {
                query = query.Where(u => u.Lastname.ToLower().Contains(findCommand.Lastname.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Username))
            {
                query = query.Where(u => u.Username.ToLower().Contains(findCommand.Username.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Email))
            {
                query = query.Where(u => u.Email.ToLower().Contains(findCommand.Email.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(findCommand.Keyword))
            {
                string keyword = findCommand.Keyword.ToLower();
                query = query.Where(u =>
                    u.Firstname.ToLower().Contains(keyword) ||
                    u.Lastname.ToLower().Contains(keyword) ||
                    u.Username.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword));
            }

            return query;
        };

        return await queryFunc(_userRepository.Table).ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task UpdateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        await _userRepository.DeleteAsync(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var cacheKey = _cacheManager.PrepareCacheKey(UserCacheDefaults.ByEmailCacheKey,
            email);

        return await _cacheManager.GetAsync(cacheKey, async () => await _userRepository.Table
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower()));
    }

    public async Task<User?> GetUserByUseranmeAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var cacheKey = _cacheManager.PrepareCacheKey(UserCacheDefaults.ByUsernameCacheKey,
            username);

        return await _cacheManager.GetAsync(cacheKey, async () => await _userRepository.Table
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower()));
    }

    public async Task AddRoleToUserAsync(int userId, int roleId)
    {
        User? user;
        if (userId <= 0 || (user = await GetUserByIdAsync(userId)) is null)
        {
            throw new EntityNotFoundException(nameof(User));
        }

        Role? role;
        if (roleId <= 0 || (role = await _roleService.GetRoleByIdAsync(roleId)) is null)
        {
            throw new EntityNotFoundException(nameof(Role));
        }

        await _roleUserMappingRepository.InsertAsync(new RoleUserMapping { UserId = user.Id, RoleId = role.Id });
    }

    public async Task<IList<Role>> GetUserRolesAsync(int userId)
    {
        if (userId <= 0)
        {
            return [];
        }

        var roleIds = await _roleUserMappingRepository.Table
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToListAsync();

        return await _roleService.GetRolesByIdsAsync(roleIds);
    }
}