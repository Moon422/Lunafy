using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Services;

public interface IRoleService
{
    Task<IList<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(int roleId);
    Task<IList<Role>> GetRolesByIdsAsync(IList<int> roleIds);
    Task CreateRoleAsync(Role role);
    Task UpdateRoleAsync(Role role);
    Task DeleteRoleAsync(Role role);
}
