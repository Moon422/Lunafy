using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task CreateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        await _userRepository.InsertAsync(user);
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
}