using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Data;

namespace Lunafy.Services;

public interface IUserService
{
    Task CreateUserAsync(User user);
    Task CreatePasswordAsync(int userId, string password);
    Task<bool> VerifyPasswordAsync(int userId, string password);
    Task<User?> GetUserByIdAsync(int id, bool includeDeleted = false);
    Task<IList<User>> GetAllUsersAsync(bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<User>> GetAllUsersAsync(int pageIndex, int pageSize, bool includeDeleted = false, bool sortByIdDesc = false);
    Task<IPagedList<User>> FindUsersAsync(FindUsersCommand findCommand, bool? deleted = false);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
}
