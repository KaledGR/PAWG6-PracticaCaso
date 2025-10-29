// AP.Models/DTOs/UserRoleDTO.cs
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class UserRoleDTO
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("roleId")]
        public int RoleId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        // Para mostrar información completa
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("roleName")]
        public string? RoleName { get; set; }
    }
}