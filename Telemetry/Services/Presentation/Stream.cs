using System;
using Microsoft.EntityFrameworkCore;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Entities;

namespace Telemetry.Services.Presentation;

public class Stream
{
    private readonly DataBaseContext _dbContext;

    public Stream(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReturnDTO> StreamData(int sessionId, int latestPacketId)
    {
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null)
        {
            return new ReturnDTO
            {
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        List<StreamTelemetryValuesDTO> streamList = new List<StreamTelemetryValuesDTO>();
        try
        {
            streamList = await _dbContext.TelemetryValues
                .Where(t => t.SessionId == sessionId && t.Id > latestPacketId)
                .Select(t => new StreamTelemetryValuesDTO{
                    Id = t.Id,
                    Timestamp = t.Timestamp,
                    Speed = t.Speed,
                    Acceleration = t.Acceleration,
                    Distance = t.Distance,
                    Voltage = t.Voltage,
                    Current = t.Current,
                    Power = t.Power,
                    Torque = t.Torque,
                    MotorTemp = t.MotorTemp,
                    ControlTemp = t.ControlTemp,
                    EngineTemp = t.EngineTemp,
                    BatteryTemp = t.BatteryTemp,
                    Throttle = t.Throttle,
                    BrakePressed = t.BrakePressed,
                    Regen = t.Regen,
                    BatteryPercentage = t.BatteryPercentage,
                    BatteryVoltage = t.BatteryVoltage,
                    FuelPercentage = t.FuelPercentage,
                    LapNumber = t.LapNumber,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude
                })
                .OrderBy(t => t.Timestamp)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            return new ReturnDTO
            {
                Message = "Error querying telemetry data: " + ex.Message,
                Data = StatusCodes.Status500InternalServerError
            };
        }

        StreamDTO streamDTO = new StreamDTO{
            Values = streamList,
            LatestPacketId = streamList.Count > 0 ? streamList.Last().Id : latestPacketId
        };
        
        return new ReturnDTO
        {
            Status = true,
            Message = "Data streamed successfully",
            Data = streamDTO
        };
    }

    public async Task<ReturnDTO> StreamLapData(int sessionId, int LapNumber){
       
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null)
        {
            return new ReturnDTO
            {
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        List<StreamTelemetryValuesDTO> streamList = new List<StreamTelemetryValuesDTO>();

        try{
            streamList = await _dbContext.TelemetryValues
                .Where(t => t.SessionId == sessionId && t.LapNumber == LapNumber)
                .Select(t => new StreamTelemetryValuesDTO{
                    Id = t.Id,
                    Timestamp = t.Timestamp,
                    Speed = t.Speed,
                    Acceleration = t.Acceleration,
                    Distance = t.Distance,
                    Voltage = t.Voltage,
                    Current = t.Current,
                    Power = t.Power,
                    Torque = t.Torque,
                    MotorTemp = t.MotorTemp,
                    ControlTemp = t.ControlTemp,
                    EngineTemp = t.EngineTemp,
                    BatteryTemp = t.BatteryTemp,
                    Throttle = t.Throttle,
                    BrakePressed = t.BrakePressed,
                    Regen = t.Regen,
                    BatteryPercentage = t.BatteryPercentage,
                    BatteryVoltage = t.BatteryVoltage,
                    FuelPercentage = t.FuelPercentage,
                    LapNumber = t.LapNumber,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude
                })
                .OrderBy(t => t.Timestamp)
                .ToListAsync();
        }catch(Exception ex){
            return new ReturnDTO
            {
                Message = "Error querying telemetry data: " + ex.Message,
                Data = StatusCodes.Status500InternalServerError
            };
        }

        if (streamList.Count == 0)
        {
            return new ReturnDTO
            {
                Message = "No telemetry data found for the specified lap",
                Data = StatusCodes.Status404NotFound
            };
        }

        StreamLapDTO streamLapDTO = new StreamLapDTO{
            Values = streamList,
            LapTime = streamList.Last().Timestamp - streamList.First().Timestamp
        };
        
        return new ReturnDTO
        {
            Status = true,
            Message = "Data streamed successfully",
            Data = streamLapDTO
        };
    }

    public async Task<ReturnDTO> GetLapNumbers(int sessionId){
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null)
        {
            return new ReturnDTO
            {
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        List<int> lapList = new List<int>();
        try{
            lapList = await _dbContext.TelemetryValues
                                    .Where(t => t.SessionId == sessionId)
                                    .OrderBy(t => t.LapNumber)
                                    .Select(t => t.LapNumber)
                                    .Distinct()
                                    .ToListAsync();
        }catch (Exception ex){
            return new ReturnDTO
            {
                Message = "Error querying telemetry data: " + ex.Message,
                Data = StatusCodes.Status500InternalServerError
            };
        }
        
        return new ReturnDTO
        {
            Status = true,
            Message = "Laps retrieved successfully",
            Data = lapList
        };
    }

    public async Task<ReturnDTO> GetLapTimes(int sessionId){
        
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null)
        {
            return new ReturnDTO
            {
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }
        
        Dictionary<int, double> laptimes = new Dictionary<int, double>();
        try{
            laptimes = await GetLapTimesDict(sessionId);

        }catch(Exception ex){
            return new ReturnDTO
            {
                Message = "Error querying telemetry data: " + ex.Message,
                Data = StatusCodes.Status500InternalServerError
            };
        }

        return new ReturnDTO
        {
            Status = true,
            Message = "Lap times retrieved successfully",
            Data = laptimes
        };
    }
    
    private async Task<Dictionary<int, double>> GetLapTimesDict(int sessionId){
        Dictionary<int, double> laptimes = new Dictionary<int, double>();
        try{
            laptimes = await _dbContext.TelemetryValues
                                    .Where(t => t.SessionId == sessionId)
                                    .OrderBy(t => t.LapNumber)
                                    .GroupBy(t => t.LapNumber)
                                    .ToDictionaryAsync(g=>g.Key, g=>g.Max(t => t.Timestamp) - g.Min(t => t.Timestamp));
        
        }catch (Exception ex){
            throw new Exception(ex.Message);
        }
        return laptimes;
    } 
}