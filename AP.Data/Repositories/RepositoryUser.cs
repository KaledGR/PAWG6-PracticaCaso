// AP.Data/Repositories/RepositoryUser.cs
using Microsoft.EntityFrameworkCore;
using AP.Data.Models;

namespace AP.Data.Repositories;

public interface IRepositoryUser : IRepositoryBase<User>
{
    /// <summary>
    /// Busca un usuario por email
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca un usuario por email con sus roles incluidos
    /// </summary>
    Task<User?> GetByEmailWithRolesAsync(string email);

    /// <summary>
    /// Obtiene todos los usuarios con sus roles
    /// </summary>
    Task<IEnumerable<User>> GetAllWithRolesAsync();

    /// <summary>
    /// Actualiza el último login del usuario
    /// </summary>
    Task<bool> UpdateLastLoginAsync(int userId);
}

public class RepositoryUser : RepositoryBase<User>, IRepositoryUser
{
    public RepositoryUser(TaskDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await DbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailWithRolesAsync(string email)
    {
        return await DbContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync()
    {
        return await DbContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<bool> UpdateLastLoginAsync(int userId)
    {
        var user = await FindAsync(userId);
        if (user == null) return false;

        user.LastLogin = DateTime.UtcNow;
        return await UpdateAsync(user);
    }

    public new async Task<bool> ExistsAsync(User entity)
    {
        return await DbContext.Users.AnyAsync(u => u.UserId == entity.UserId);
    }
}