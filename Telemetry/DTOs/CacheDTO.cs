namespace Telemetry.DTOs;

public record class CacheDTO
{
    public int Speed { get; set; }
    public double Timestamp { get; set; }
    public int LapNumber { get; set; }
    public float? CurrentBatteryPercentage { get; set; }
    public float BatteryCapacity { get; set; }
    public double Distance { get; set; }
    public DateTime ExpiresAt { get; set; } = DateTime.Now.AddHours(1);
}