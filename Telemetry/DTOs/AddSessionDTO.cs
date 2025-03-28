namespace Telemetry.DTOs;

public record class AddSessionDTO
{
    public required string VehicleName { get; set; }
    public required string Password { get; set; }

    public string SessionName { get; set; } = "DFLT";
    public string TrackName { get; set; } = "DFLT";
    
    public float? BatteryPercentage { get; set; }
    public float? BatteryCapacity { get; set; }

    public int CacheTime { get; set; } = 1;

}
