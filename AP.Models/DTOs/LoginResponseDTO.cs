// AP.Models/DTOs/LoginResponseDTO.cs
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class LoginResponseDTO
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;

        [JsonPropertyName("user")]
        public UserDTO? User { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }  // Por si quieres usar JWT después
    }
}