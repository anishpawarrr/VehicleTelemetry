using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Entities;

namespace Telemetry.Services.Setup;

public class SessionSetup : ISetup<AddSessionDTO, int, UpdateSessionDTO, int>
{
    private readonly DataBaseContext _dbContext;

    public SessionSetup(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReturnDTO> Create(AddSessionDTO sessionDTO)
    {
        Vehicle? vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Name == sessionDTO.VehicleName && v.Password == sessionDTO.Password);
        if (vehicle is null){
            return new ReturnDTO{
                Message = "Vehicle name, password or both are incorrect",
                Data = StatusCodes.Status400BadRequest
            };
        }

        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Name == sessionDTO.SessionName && s.VehicleId == vehicle.Id);
        if (session is not null){
            return new ReturnDTO{
                Message = "Session name already exists",
                Data = StatusCodes.Status400BadRequest
            };
        }

        Session newSession = new Session{
            Name = sessionDTO.SessionName,
            TrackName = sessionDTO.TrackName,
            VehicleId = vehicle.Id
        };

        await _dbContext.Sessions.AddAsync(newSession);
        await _dbContext.SaveChangesAsync();

        CacheDTO cacheDTO = new CacheDTO{
            Speed = 0,
            Timestamp = 0,
            LapNumber = 0,
            CurrentBatteryPercentage = sessionDTO.BatteryPercentage,
            BatteryCapacity = sessionDTO.BatteryCapacity is null ? 0 : sessionDTO.BatteryCapacity.Value,
            Distance = 0,
            ExpiresAt = DateTime.Now.AddHours(sessionDTO.CacheTime)
        };
        Cache.Set(newSession.Id, cacheDTO);
        
        return new ReturnDTO{
            Status = true,
            Message = "Session created successfully",
            Data = newSession.Id
        };
    }

    public async Task<ReturnDTO> Retrieve(int id)
    {
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == id);
        if (session is null){
            return new ReturnDTO{
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        return new ReturnDTO{
            Status = true,
            Message = "Session retrieved successfully",
            Data = new {
                Id = session.Id,
                Name = session.Name,
                TrackName = session.TrackName,
                CreatedAt = session.CreatedAt,
                VehicleId = session.VehicleId
            }
        };
    }

    public async Task<ReturnDTO> Update(UpdateSessionDTO sessionDTO)
    {
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionDTO.SessionId);
        if (session is null){
            return new ReturnDTO{
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        session.Name = sessionDTO.SessionName;
        session.TrackName = sessionDTO.TrackName;

        await _dbContext.SaveChangesAsync();

        return new ReturnDTO{
            Status = true,
            Message = "Session updated successfully",
            Data = session.Id
        };
    }

    public async Task<ReturnDTO> Delete(int id)
    {
        Session? session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == id);
        if (session is null){
            return new ReturnDTO{
                Message = "Session not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync();


        return new ReturnDTO{
            Status = true,
            Message = "Session deleted successfully",
            Data = session.Id
        };
    }

}