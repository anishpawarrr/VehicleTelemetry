namespace Telemetry.DTOs;

public record class UpdateSessionDTO
{
    public required int SessionId { get; set; }
    public string SessionName { get; set; } = "DFLT";
    public string TrackName { get; set; } = "DFLT";
}
