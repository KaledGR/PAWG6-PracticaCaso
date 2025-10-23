// AP.Models/DTOs/LoginDTO.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;
    }
}