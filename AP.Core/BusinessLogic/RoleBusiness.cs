// AP.Core/BusinessLogic/RoleBusiness.cs
using AP.Data.Models;
using AP.Data.Repositories;

namespace AP.Core.BusinessLogic
{
    public interface IRoleBusiness
    {
        /// <summary>
        /// Obtiene todos los roles
        /// </summary>
        Task<IEnumerable<Role>> GetAllRolesAsync();

        /// <summary>
        /// Obtiene un rol por ID
        /// </summary>
        Task<Role?> GetRoleByIdAsync(int roleId);

        /// <summary>
        /// Asigna un rol a un usuario
        /// </summary>
        Task<bool> AssignRoleToUserAsync(int userId, int roleId, string? description = null);

        /// <summary>
        /// Actualiza el rol de un usuario (cambia de un rol a otro)
        /// </summary>
        Task<bool> UpdateUserRoleAsync(int userId, int newRoleId, string? description = null);

        /// <summary>
        /// Obtiene todos los usuarios con sus roles asignados
        /// </summary>
        Task<IEnumerable<UserRole>> GetAllUserRolesAsync();

        /// <summary>
        /// Verifica si un usuario tiene un rol asignado
        /// </summary>
        Task<bool> UserHasRoleAsync(int userId);

        /// <summary>
        /// Valida que se pueda asignar un rol a un usuario
        /// </summary>
        Task<(bool IsValid, string ErrorMessage)> ValidateRoleAssignmentAsync(int userId, int roleId);
    }

    public class RoleBusiness : IRoleBusiness
    {
        private readonly IRepositoryRole _roleRepository;
        private readonly IRepositoryUserRole _userRoleRepository;
        private readonly IRepositoryUser _userRepository;

        public RoleBusiness(
            IRepositoryRole roleRepository,
            IRepositoryUserRole userRoleRepository,
            IRepositoryUser userRepository)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.ReadAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _roleRepository.FindAsync(roleId);
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId, string? description = null)
        {
            // Validar antes de asignar
            var validation = await ValidateRoleAssignmentAsync(userId, roleId);
            if (!validation.IsValid)
            {
                return false;
            }

            return await _userRoleRepository.AssignRoleAsync(userId, roleId, description);
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int newRoleId, string? description = null)
        {
            // Validar que el usuario existe
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Validar que el rol existe
            var role = await _roleRepository.FindAsync(newRoleId);
            if (role == null)
            {
                return false;
            }

            return await _userRoleRepository.UpdateUserRoleAsync(userId, newRoleId, description);
        }

        public async Task<IEnumerable<UserRole>> GetAllUserRolesAsync()
        {
            return await _userRoleRepository.GetAllWithDetailsAsync();
        }

        public async Task<bool> UserHasRoleAsync(int userId)
        {
            return await _userRoleRepository.UserHasRoleAsync(userId);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateRoleAssignmentAsync(int userId, int roleId)
        {
            // Validar que el usuario existe
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                return (false, "El usuario no existe");
            }

            // Validar que el usuario está activo
            if (!user.IsActive)
            {
                return (false, "El usuario no está activo");
            }

            // Validar que el rol existe
            var role = await _roleRepository.FindAsync(roleId);
            if (role == null)
            {
                return (false, "El rol no existe");
            }

            // Validar que el usuario no tenga ya un rol asignado (solo puede tener uno)
            var hasRole = await _userRoleRepository.UserHasRoleAsync(userId);
            if (hasRole)
            {
                return (false, "El usuario ya tiene un rol asignado. Use UpdateUserRole para cambiarlo.");
            }

            return (true, string.Empty);
        }
    }
}