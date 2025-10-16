using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AP.Models.DTOs
{
    public class TaskDTO
    {
        [JsonPropertyName("taskId")]
        public int TaskId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("createDate")]
        public DateTime CreatedDate { get; set; }
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = null!;
    }
}
