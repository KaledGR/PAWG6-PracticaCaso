// AP.Data/Repositories/RepositoryRole.cs
using Microsoft.EntityFrameworkCore;
using AP.Data.Models;

namespace AP.Data.Repositories;

public interface IRepositoryRole : IRepositoryBase<Role>
{
    /// <summary>
    /// Busca un rol por su nombre
    /// </summary>
    Task<Role?> GetByNameAsync(string roleName);

    /// <summary>
    /// Verifica si existe un rol por nombre
    /// </summary>
    Task<bool> ExistsByNameAsync(string roleName);
}

public class RepositoryRole : RepositoryBase<Role>, IRepositoryRole
{
    public RepositoryRole(TaskDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await DbContext.Roles
            .FirstOrDefaultAsync(r => r.RoleName == roleName);
    }

    public async Task<bool> ExistsByNameAsync(string roleName)
    {
        return await DbContext.Roles
            .AnyAsync(r => r.RoleName == roleName);
    }

    public new async Task<bool> ExistsAsync(Role entity)
    {
        return await DbContext.Roles.AnyAsync(r => r.RoleId == entity.RoleId);
    }
}