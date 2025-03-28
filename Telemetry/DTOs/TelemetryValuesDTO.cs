namespace Telemetry.DTOs;

public record class TelemetryValuesDTO
{
    // [System.Text.Json.Serialization.JsonPropertyName("timestamp")]
    public required double Timestamp { get; set; }
    public double? PreviousTimestamp { get; set; }

    public int Speed { get; set; } = 0;
    public int? PreviousSpeed { get; set; }
    public double? Acceleration { get; set; }
    public double? Distance { get; set; }

    public int Voltage { get; set; } = 0;
    public int Current { get; set; } = 0;

    public int? Power { get; set; }

    public int Torque { get; set; } = 0;

    public int Motor_temp { get; set; } = 0;
    public int Control_temp { get; set; } = 0;
    public int Engine_temp { get; set; } = 0;
    public int Battery_temp { get; set; } = 0;

    public int Throttle { get; set; } = 0;
    public bool BrakePressed { get; set; } = false;

    public int Regen { get; set; } = 0;

    // [System.Text.Json.Serialization.JsonPropertyName("sessionId")]
    public required int SessionId { get; set; }

    public int BatteryVoltage { get; set; }
    public float? BatteryPercentage { get; set; }
    public int FuelPercentage { get; set; }

    public int? LapNumber { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}