namespace Telemetry.DTOs;

public record class UpdateVehicleDTO
{
    public required string VehicleName { get; set; }
    public required string Password { get; set; }
    public required string NewVehicleName { get; set; }
    public required string NewPassword { get; set; }
}
