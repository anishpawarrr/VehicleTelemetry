namespace Telemetry.DTOs;

public record class StreamDTO
{
    public List<StreamTelemetryValuesDTO> Values { get; set; } = [];
    public int LatestPacketId { get; set; } = 0;
}
