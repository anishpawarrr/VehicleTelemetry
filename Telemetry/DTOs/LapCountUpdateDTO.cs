namespace Telemetry.DTOs;

public record class LapCountUpdateDTO
{
    public int LapNumber { get; set; } = -1;
    public required int SessionId { get; set; }
}
