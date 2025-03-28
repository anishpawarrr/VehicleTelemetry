namespace Telemetry.DTOs;

public record class StreamTelemetryValuesDTO
{
    public required int Id { get; set; }
    public double Timestamp { get; set; }
    
    public int Speed { get; set; }
    public double Acceleration { get; set; }
    public double Distance { get; set; }

    public int Voltage { get; set; }
    public int Current { get; set; }

    public int Power { get; set; }

    public int Torque { get; set; }

    public int MotorTemp { get; set; }
    public int ControlTemp { get; set; }
    public int EngineTemp { get; set; }
    public int BatteryTemp { get; set; }

    public int Throttle { get; set; }
    public bool BrakePressed { get; set; }
    
    public int Regen { get; set; }

    public float? BatteryPercentage { get; set; }
    public int BatteryVoltage { get; set; }
    public int FuelPercentage { get; set; }

    public int LapNumber { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }    
}
