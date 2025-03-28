using Microsoft.VisualBasic;

namespace Telemetry.DTOs;

public record class ReturnDTO
{
    public bool Status { get; set; } = false;
    public string? Message { get; set; }
    public object? Data { get; set; }
}
