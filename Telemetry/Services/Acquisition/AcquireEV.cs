using System;
using Microsoft.EntityFrameworkCore;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Entities;

namespace Telemetry.Services.Acquisition;

public class AcquireEV : IAcquire
{
    private readonly DataBaseContext _dbContext;
    public AcquireEV(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ReturnDTO> AcquireValues(TelemetryValuesDTO telemetryValuesDTO)
    {

        // // System.Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
        // System.Console.WriteLine($"\n\nThread ID: {Thread.CurrentThread.ManagedThreadId}, Thread Name: {Thread.CurrentThread.Name}\n\n");

        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == telemetryValuesDTO.SessionId);
        if (session == null)
        {
            return new ReturnDTO
            {
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        CacheDTO cacheDTO = Cache.Get(telemetryValuesDTO.SessionId);

        telemetryValuesDTO.PreviousTimestamp ??= cacheDTO.Timestamp;
        telemetryValuesDTO.PreviousSpeed ??= cacheDTO.Speed;
        telemetryValuesDTO.LapNumber ??= cacheDTO.LapNumber;
        telemetryValuesDTO.Latitude ??= 0;
        telemetryValuesDTO.Longitude ??= 0;
        telemetryValuesDTO.Power ??= telemetryValuesDTO.Voltage * telemetryValuesDTO.Current;

        try{

            telemetryValuesDTO.Acceleration ??= GetAcceleration(previousSpeed: telemetryValuesDTO.PreviousSpeed.Value, 
                                                                currentSpeed: telemetryValuesDTO.Speed, 
                                                                previousTimestamp: telemetryValuesDTO.PreviousTimestamp.Value, 
                                                                currentTimestamp: telemetryValuesDTO.Timestamp);
            
            telemetryValuesDTO.Distance ??= GetDistance(previousSpeed: telemetryValuesDTO.PreviousSpeed.Value,
                                                    previousTimestamp: telemetryValuesDTO.PreviousTimestamp.Value,
                                                    currentTimestamp: telemetryValuesDTO.Timestamp,
                                                    acceleration: telemetryValuesDTO.Acceleration.Value) + cacheDTO.Distance;

            if (telemetryValuesDTO.BatteryPercentage is null && cacheDTO.CurrentBatteryPercentage is not null){
                telemetryValuesDTO.BatteryPercentage = GetBatteryPercentage(currentBatteryPercentage: cacheDTO.CurrentBatteryPercentage.Value, 
                                                                            current: telemetryValuesDTO.Current, 
                                                                            batteryCapacity: cacheDTO.BatteryCapacity, 
                                                                            previousTimestamp: telemetryValuesDTO.PreviousTimestamp.Value, 
                                                                            currentTimestamp: telemetryValuesDTO.Timestamp);
            }

            cacheDTO.Timestamp = telemetryValuesDTO.Timestamp;
            cacheDTO.Speed = telemetryValuesDTO.Speed;
            cacheDTO.CurrentBatteryPercentage = telemetryValuesDTO.BatteryPercentage;
            cacheDTO.Distance = telemetryValuesDTO.Distance.Value;
            Cache.Set(telemetryValuesDTO.SessionId, cacheDTO);

        }catch (Exception ex){
            return new ReturnDTO{
                Message = $"Error in calculating values: {ex.Message}",
                Data = StatusCodes.Status500InternalServerError
            };
        }

        TelemetryValue telemetryValue = new TelemetryValue{
            Speed = telemetryValuesDTO.Speed,
            Timestamp = telemetryValuesDTO.Timestamp,
            Acceleration = telemetryValuesDTO.Acceleration.Value,
            Distance = telemetryValuesDTO.Distance.Value,
            Voltage = telemetryValuesDTO.Voltage,
            Current = telemetryValuesDTO.Current,
            Power = telemetryValuesDTO.Power.Value,
            Torque = telemetryValuesDTO.Torque,
            MotorTemp = telemetryValuesDTO.Motor_temp,
            ControlTemp = telemetryValuesDTO.Control_temp,
            EngineTemp = telemetryValuesDTO.Engine_temp,
            BatteryTemp = telemetryValuesDTO.Battery_temp,
            Throttle = telemetryValuesDTO.Throttle,
            BrakePressed = telemetryValuesDTO.BrakePressed,
            Regen = telemetryValuesDTO.Regen,
            BatteryPercentage = telemetryValuesDTO.BatteryPercentage,
            BatteryVoltage = telemetryValuesDTO.BatteryVoltage,
            FuelPercentage = telemetryValuesDTO.FuelPercentage,
            LapNumber = telemetryValuesDTO.LapNumber.Value,
            Latitude = telemetryValuesDTO.Latitude.Value,
            Longitude = telemetryValuesDTO.Longitude.Value,
            SessionId = telemetryValuesDTO.SessionId
        };


        await _dbContext.TelemetryValues.AddAsync(telemetryValue);
        await _dbContext.SaveChangesAsync();
        Console.WriteLine($"\n\nThread: {Task.CurrentId}\nTime: {DateTime.Now}\nValues: {telemetryValuesDTO}\n\n");

        return new ReturnDTO{
            Status = true,
            Message = "Values Acquired and Saved",
            Data = StatusCodes.Status201Created
        };
    }

    public async Task<ReturnDTO> AcquireValuesInBatch(TelemetryValuesBatchDTO telemetryValuesBatchDTO)
    {
        foreach (TelemetryValuesDTO telemetryValuesDTO in telemetryValuesBatchDTO.Values)
        {
            ReturnDTO returnDTO = await AcquireValues(telemetryValuesDTO);
            if (!returnDTO.Status)
            {
                return returnDTO;
            }
        }
        return new ReturnDTO
        {
            Status = true,
            Message = "Batch Values Acquired and Saved",
            Data = StatusCodes.Status201Created
        };
    }
    
    public async Task<ReturnDTO> LapUpdate(LapCountUpdateDTO lapCountUpdateDTO)
    {
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == lapCountUpdateDTO.SessionId);
        if (session is null)
        {
            return new ReturnDTO
            {
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        CacheDTO cacheDTO = Cache.Get(lapCountUpdateDTO.SessionId);
        cacheDTO.LapNumber = lapCountUpdateDTO.LapNumber == -1 ? cacheDTO.LapNumber + 1 : lapCountUpdateDTO.LapNumber;
        Cache.Set(lapCountUpdateDTO.SessionId, cacheDTO);

        return new ReturnDTO
        {
            Status = true,
            Message = "Lap Incremented",
            Data = cacheDTO.LapNumber
        };   
    }

    private float GetBatteryPercentage(float currentBatteryPercentage, int current, float batteryCapacity, double previousTimestamp, double currentTimestamp)
    {
        float timeDiff =  (float)(currentTimestamp - previousTimestamp)/3600.0f;
        float energyConsumed = current * timeDiff;
        float drop = 100f * energyConsumed / batteryCapacity;
        float batteryPercentage = currentBatteryPercentage - drop;
        return batteryPercentage < 0 ? 0 : batteryPercentage;
    }

    private double GetAcceleration(int previousSpeed, int currentSpeed, double previousTimestamp, double currentTimestamp)
    {
        return (currentSpeed - previousSpeed) / (currentTimestamp - previousTimestamp);
    }

    private double GetDistance(int previousSpeed, double previousTimestamp, double currentTimestamp, double acceleration)
    {
        double timeDiff = currentTimestamp - previousTimestamp;
        return  previousSpeed * timeDiff + 0.5 * acceleration * timeDiff * timeDiff;
    }

    public Task<ReturnDTO> TurnMetrics(TurnMetricsDTO turnMetricsDTO)
    {
        throw new NotImplementedException();
    }

}
