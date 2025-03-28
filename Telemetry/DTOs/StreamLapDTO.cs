namespace Telemetry.DTOs;

public record class StreamLapDTO
{
    public List<StreamTelemetryValuesDTO> Values { get; set; } = [];
    public double LapTime { get; set; }
}
