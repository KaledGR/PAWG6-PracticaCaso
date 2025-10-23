// AP.Core/BusinessLogic/AuthBusiness.cs
using AP.Data.Models;
using AP.Data.Repositories;

namespace AP.Core.BusinessLogic
{
    public interface IAuthBusiness
    {
        /// <summary>
        /// Valida las credenciales del usuario (email y password)
        /// </summary>
        Task<User?> ValidateUserAsync(string email, string password);

        /// <summary>
        /// Obtiene un usuario con sus roles
        /// </summary>
        Task<User?> GetUserWithRolesAsync(string email);

        /// <summary>
        /// Verifica si un usuario tiene un rol específico
        /// </summary>
        Task<bool> HasRoleAsync(int userId, string roleName);

        /// <summary>
        /// Verifica si un usuario es Manager o Admin
        /// </summary>
        Task<bool> IsManagerOrAdminAsync(int userId);
    }

    public class AuthBusiness : IAuthBusiness
    {
        private readonly IRepositoryUser _userRepository;
        private readonly IRepositoryUserRole _userRoleRepository;

        public AuthBusiness(IRepositoryUser userRepository, IRepositoryUserRole userRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            // NOTA: En este caso práctico NO hay campo password en la BD
            // Por lo tanto, solo validamos que el usuario exista y esté activo
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Actualizar último login
            await _userRepository.UpdateLastLoginAsync(user.UserId);

            return user;
        }

        public async Task<User?> GetUserWithRolesAsync(string email)
        {
            return await _userRepository.GetByEmailWithRolesAsync(email);
        }

        public async Task<bool> HasRoleAsync(int userId, string roleName)
        {
            return await _userRoleRepository.HasRoleAsync(userId, roleName);
        }

        public async Task<bool> IsManagerOrAdminAsync(int userId)
        {
            var isManager = await _userRoleRepository.HasRoleAsync(userId, "Manager");
            var isAdmin = await _userRoleRepository.HasRoleAsync(userId, "Admin");

            return isManager || isAdmin;
        }
    }
}