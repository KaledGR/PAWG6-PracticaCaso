using System;
using System.Collections.Generic;

namespace AP.Data.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Approved { get; set; }
}
