// AP.Mvc/Helpers/SessionHelper.cs
using AP.Models.DTOs;
using System.Text.Json;

namespace AP.Mvc.Helpers
{
    public static class SessionHelper
    {
        private const string USER_SESSION_KEY = "CurrentUser";

        /// <summary>
        /// Guarda el usuario en sesión
        /// </summary>
        public static void SetUser(ISession session, UserDTO user)
        {
            var userJson = JsonSerializer.Serialize(user);
            session.SetString(USER_SESSION_KEY, userJson);
        }

        /// <summary>
        /// Obtiene el usuario de la sesión
        /// </summary>
        public static UserDTO? GetUser(ISession session)
        {
            var userJson = session.GetString(USER_SESSION_KEY);
            if (string.IsNullOrEmpty(userJson))
                return null;

            return JsonSerializer.Deserialize<UserDTO>(userJson);
        }

        /// <summary>
        /// Verifica si hay un usuario logueado
        /// </summary>
        public static bool IsAuthenticated(ISession session)
        {
            return !string.IsNullOrEmpty(session.GetString(USER_SESSION_KEY));
        }

        /// <summary>
        /// Cierra la sesión
        /// </summary>
        public static void ClearSession(ISession session)
        {
            session.Clear();
        }

        /// <summary>
        /// Verifica si el usuario es Manager o Admin
        /// </summary>
        public static bool IsManagerOrAdmin(ISession session)
        {
            var user = GetUser(session);
            if (user == null) return false;

            return user.RoleName == "Manager" || user.RoleName == "Admin";
        }
    }
}