// AP.Data/Repositories/RepositoryUserRole.cs
using Microsoft.EntityFrameworkCore;
using AP.Data.Models;

namespace AP.Data.Repositories;

public interface IRepositoryUserRole
{
    /// <summary>
    /// Asigna un rol a un usuario
    /// </summary>
    Task<bool> AssignRoleAsync(int userId, int roleId, string? description = null);

    /// <summary>
    /// Remueve un rol de un usuario
    /// </summary>
    Task<bool> RemoveRoleAsync(int userId, int roleId);

    /// <summary>
    /// Obtiene el rol de un usuario
    /// </summary>
    Task<UserRole?> GetUserRoleAsync(int userId);

    /// <summary>
    /// Obtiene el rol de un usuario con información completa
    /// </summary>
    Task<UserRole?> GetUserRoleWithDetailsAsync(int userId);

    /// <summary>
    /// Verifica si un usuario tiene un rol específico
    /// </summary>
    Task<bool> HasRoleAsync(int userId, string roleName);

    /// <summary>
    /// Obtiene todos los usuarios con sus roles
    /// </summary>
    Task<IEnumerable<UserRole>> GetAllWithDetailsAsync();

    /// <summary>
    /// Actualiza el rol de un usuario (remueve el anterior y asigna el nuevo)
    /// </summary>
    Task<bool> UpdateUserRoleAsync(int userId, int newRoleId, string? description = null);

    /// <summary>
    /// Verifica si un usuario ya tiene un rol asignado
    /// </summary>
    Task<bool> UserHasRoleAsync(int userId);
}

public class RepositoryUserRole : IRepositoryUserRole
{
    private readonly TaskDbContext _context;

    public RepositoryUserRole(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AssignRoleAsync(int userId, int roleId, string? description = null)
    {
        try
        {
            // Verificar si ya tiene un rol asignado
            var existingRole = await GetUserRoleAsync(userId);
            if (existingRole != null)
            {
                return false; // Ya tiene un rol
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                Description = description
            };

            await _context.UserRoles.AddAsync(userRole);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveRoleAsync(int userId, int roleId)
    {
        try
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null) return false;

            _context.UserRoles.Remove(userRole);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<UserRole?> GetUserRoleAsync(int userId)
    {
        return await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
    }

    public async Task<UserRole?> GetUserRoleWithDetailsAsync(int userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
    }

    public async Task<bool> HasRoleAsync(int userId, string roleName)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.RoleName == roleName);
    }

    public async Task<IEnumerable<UserRole>> GetAllWithDetailsAsync()
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => ur.User.IsActive)
            .OrderBy(ur => ur.User.Username)
            .ToListAsync();
    }

    public async Task<bool> UpdateUserRoleAsync(int userId, int newRoleId, string? description = null)
    {
        try
        {
            // Buscar el rol actual del usuario
            var existingRole = await GetUserRoleAsync(userId);

            if (existingRole == null)
            {
                // Si no tiene rol, asignar el nuevo
                return await AssignRoleAsync(userId, newRoleId, description);
            }

            // Si ya tiene el mismo rol, solo actualizar descripción
            if (existingRole.RoleId == newRoleId)
            {
                existingRole.Description = description;
                _context.UserRoles.Update(existingRole);
            }
            else
            {
                // Remover el rol actual y asignar el nuevo
                _context.UserRoles.Remove(existingRole);

                var newUserRole = new UserRole
                {
                    UserId = userId,
                    RoleId = newRoleId,
                    Description = description
                };

                await _context.UserRoles.AddAsync(newUserRole);
            }

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UserHasRoleAsync(int userId)
    {
        return await _context.UserRoles.AnyAsync(ur => ur.UserId == userId);
    }
}
