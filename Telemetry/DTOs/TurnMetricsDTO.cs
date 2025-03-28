namespace Telemetry.DTOs;

public record class TurnMetricsDTO
{
    public List<Dictionary<string, double>> Metrics { get; set; } = [];
    public int SessionId { get; set; }
    public int LapNumber { get; set; }
}