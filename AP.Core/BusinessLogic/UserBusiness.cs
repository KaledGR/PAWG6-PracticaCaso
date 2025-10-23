// AP.Core/BusinessLogic/UserBusiness.cs
using AP.Data.Models;
using AP.Data.Repositories;

namespace AP.Core.BusinessLogic
{
    public interface IUserBusiness
    {
        /// <summary>
        /// Obtiene todos los usuarios activos
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Obtiene todos los usuarios con sus roles
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersWithRolesAsync();

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        Task<User?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Obtiene un usuario por email
        /// </summary>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        Task<bool> CreateUserAsync(User user);

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        Task<bool> UpdateUserAsync(User user);
    }

    public class UserBusiness : IUserBusiness
    {
        private readonly IRepositoryUser _userRepository;

        public UserBusiness(IRepositoryUser userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.ReadAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersWithRolesAsync()
        {
            return await _userRepository.GetAllWithRolesAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.FindAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            // Validaciones
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Username))
            {
                return false;
            }

            // Verificar que el email no exista
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return false;
            }

            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.FindAsync(user.UserId);
            if (existingUser == null)
            {
                return false;
            }

            return await _userRepository.UpdateAsync(user);
        }
    }
}