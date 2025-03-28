using System;
using Microsoft.EntityFrameworkCore;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Entities;

namespace Telemetry.Services.Setup;

public class VehicleSetup : ISetup<VehicleDTO, VehicleDTO, UpdateVehicleDTO, VehicleDTO>
{
    private readonly DataBaseContext _dbContext;

    public VehicleSetup(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReturnDTO> Create(VehicleDTO vehicleDTO)
    {
        Vehicle? vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Name == vehicleDTO.VehicleName);
        if (vehicle is not null)
        {
            return new ReturnDTO
            {
                Message = "Vehicle name already exists",
                Data = StatusCodes.Status400BadRequest
            };
        }

        Vehicle newVehicle = new Vehicle
        {
            Name = vehicleDTO.VehicleName,
            Password = vehicleDTO.Password
        };

        await _dbContext.Vehicles.AddAsync(newVehicle);
        await _dbContext.SaveChangesAsync();

        return new ReturnDTO
        {
            Status = true,
            Message = "Vehicle added successfully",
            Data = newVehicle.Id
        };
    }

    public async Task<ReturnDTO> Retrieve(VehicleDTO retrieveVehicleDTO)
    {
        Vehicle? vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Name == retrieveVehicleDTO.VehicleName && v.Password == retrieveVehicleDTO.Password);
        if (vehicle is null)
        {
            return new ReturnDTO
            {
                Message = "Vehicle name or password is incorrect",
                Data = StatusCodes.Status404NotFound
            };
        }

        return new ReturnDTO
        {
            Status = true,
            Message = "Vehicle found",
            Data = vehicle.Id
        };
    }

    public async Task<ReturnDTO> Update(UpdateVehicleDTO updateVehicleDTO)
    {
        Vehicle? vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Name == updateVehicleDTO.VehicleName && v.Password == updateVehicleDTO.Password);
        if (vehicle is null)
        {
            return new ReturnDTO
            {
                Message = "Vehicle not found",
                Data = StatusCodes.Status404NotFound
            };
        }

        if (updateVehicleDTO.NewVehicleName != vehicle.Name)
        {
            Vehicle? existingVehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Name == updateVehicleDTO.NewVehicleName);
            if (existingVehicle is not null)
            {
                return new ReturnDTO
                {
                    Message = "Proposed vehicle name already exists",
                    Data = StatusCodes.Status400BadRequest
                };
            }
        }

        vehicle.Name = updateVehicleDTO.NewVehicleName;
        vehicle.Password = updateVehicleDTO.NewPassword;

        await _dbContext.SaveChangesAsync();

        return new ReturnDTO
        {
            Status = true,
            Message = "Vehicle updated successfully",
            Data = vehicle.Id
        };
    }

    public async Task<ReturnDTO> Delete(VehicleDTO deleteVehicleDTO)
    {
        Vehicle? vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Name == deleteVehicleDTO.VehicleName && v.Password == deleteVehicleDTO.Password);
        if (vehicle is null)
        {
            return new ReturnDTO
            {
                Message = "Vehicle name or password is incorrect",
                Data = StatusCodes.Status404NotFound
            };
        }

        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync();

        return new ReturnDTO
        {
            Status = true,
            Message = "Vehicle deleted successfully",
            Data = vehicle.Id
        };
    }

    public async Task<ReturnDTO> GetSessionsByVehicleId(int vehicleId)
    {
        List<int> sessions = await _dbContext.Sessions
                                                    .Where(s => s.VehicleId == vehicleId)
                                                    .Select(s => s.Id)
                                                    .ToListAsync();
        
        if (sessions is null || sessions.Count == 0){
            return new ReturnDTO{
                Message = "No sessions found for this vehicle",
                Data = StatusCodes.Status404NotFound
            };
        }

        return new ReturnDTO{
            Status = true,
            Message = "Sessions retrieved successfully",
            Data = sessions
        };
    }
}