namespace Telemetry.DTOs;

public record class VehicleDTO
{
    public required string VehicleName { get; set; }
    public required string Password { get; set; }

}
