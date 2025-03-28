using System;
using Microsoft.AspNetCore.Mvc;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Services.Setup;

namespace Telemetry.APIs.V1;

public static class VehicleAPI
{
    public static WebApplication MapVehicle (this WebApplication app)
    {
        var vehicle = app.MapGroup("v1/vehicle");

        vehicle.MapPost("", async ([FromBody] VehicleDTO addVehicleDTO, DataBaseContext dbContext) => {
            
            VehicleSetup vehicleService = new VehicleSetup(dbContext);
            ReturnDTO returnDTO = await vehicleService.Create(addVehicleDTO);

            if (!returnDTO.Status)
            {
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Created(uri: "v1/vehicle", value: returnDTO.Data);
        });

        vehicle.MapGet("", async ([FromBody] VehicleDTO vehicleDTO, DataBaseContext dbContext) => {
            
            VehicleSetup vehicleService = new VehicleSetup(dbContext);
            ReturnDTO returnDTO = await vehicleService.Retrieve(vehicleDTO);

            if (!returnDTO.Status)
            {
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Ok(returnDTO.Data);
        });

        vehicle.MapPut("", async ([FromBody] UpdateVehicleDTO updateVehicleDTO, DataBaseContext dbContext) => {
            
            VehicleSetup vehicleService = new VehicleSetup(dbContext);
            ReturnDTO returnDTO = await vehicleService.Update(updateVehicleDTO);

            if (!returnDTO.Status)
            {
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Ok(returnDTO.Data);
        });

        vehicle.MapDelete("", async ([FromBody] VehicleDTO vehicleDTO, DataBaseContext dbContext) => {
            
            VehicleSetup vehicleService = new VehicleSetup(dbContext);
            ReturnDTO returnDTO = await vehicleService.Delete(vehicleDTO);

            if (!returnDTO.Status)
            {
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Ok(returnDTO.Data);
        });

        vehicle.MapGet("{Id}/sessions", async ([FromRoute] int Id, DataBaseContext dbContext) => {
            
            VehicleSetup vehicleSetup = new VehicleSetup(dbContext);
            ReturnDTO returnDTO = await vehicleSetup.GetSessionsByVehicleId(Id);

            if (!returnDTO.Status)
            {
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Ok(returnDTO.Data);
        });

        return app;
    }
}
