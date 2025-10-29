// AP.Models/DTOs/ApproveTaskDTO.cs
using System.Text.Json.Serialization;

namespace AP.Models.DTOs
{
    public class ApproveTaskDTO
    {
        [JsonPropertyName("taskId")]
        public int TaskId { get; set; }

        [JsonPropertyName("approved")]
        public bool Approved { get; set; }  // true = aprobar, false = denegar

        [JsonPropertyName("approvedBy")]
        public int ApprovedBy { get; set; }  // UserId del manager que aprueba
    }
}