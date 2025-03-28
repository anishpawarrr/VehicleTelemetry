using System;

namespace Telemetry.Entities;

public class Session
{
    public int Id { get; set; }
    public string? Name { get; set; } = "DFLT";
    public string? TrackName { get; set; } = "DFLT";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public required int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
}
