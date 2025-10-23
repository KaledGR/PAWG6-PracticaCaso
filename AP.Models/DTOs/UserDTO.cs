// AP.Models/DTOs/UserDTO.cs
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class UserDTO
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("lastLogin")]
        public DateTime? LastLogin { get; set; }

        // Para mostrar el rol asignado
        [JsonPropertyName("roleName")]
        public string? RoleName { get; set; }

        [JsonPropertyName("roleId")]
        public int? RoleId { get; set; }
    }
}