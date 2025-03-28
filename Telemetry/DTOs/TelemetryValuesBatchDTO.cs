namespace Telemetry.DTOs;

public record class TelemetryValuesBatchDTO
{
    public required List<TelemetryValuesDTO> Values { get; set; }
}
