// AP.Models/DTOs/AssignRoleDTO.cs
using AP.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class AssignRoleDTO
    {
        [Required]
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [Required]
        [JsonPropertyName("roleId")]
        public int RoleId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}