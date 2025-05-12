using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;

namespace Lunafy.Services;

[ScopeDependency(typeof(IRoleService))]
public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepository;

    public RoleService(IRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IList<Role>> GetAllRolesAsync()
    {
        return await _roleRepository.GetAllAsync(getCacheKey: cache => default);
    }

    public async Task<Role?> GetRoleByIdAsync(int roleId)
    {
        if (roleId <= 0)
            return null;

        return await _roleRepository.GetByIdAsync(roleId, cache => default);
    }

    public async Task<IList<Role>> GetRolesByIdsAsync(IList<int> roleIds)
    {
        if (roleIds is null || !roleIds.Any())
            return [];

        return await _roleRepository.GetByIdsAsync(roleIds, cache => default);
    }

    public async Task CreateRoleAsync(Role role)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));
        await _roleRepository.InsertAsync(role);
    }

    public async Task UpdateRoleAsync(Role role)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));
        await _roleRepository.UpdateAsync(role);
    }

    public async Task DeleteRoleAsync(Role role)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));
        await _roleRepository.DeleteAsync(role);
    }
}