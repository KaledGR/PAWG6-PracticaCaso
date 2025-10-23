// AP.Models/DTOs/TaskDTO.cs
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class TaskDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("approved")]
        public bool? Approved { get; set; }  // ✅ NUEVO CAMPO
    }
}