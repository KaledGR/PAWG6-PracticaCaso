// AP.Models/DTOs/RoleDTO.cs
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class RoleDTO
    {
        [JsonPropertyName("roleId")]
        public int RoleId { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}