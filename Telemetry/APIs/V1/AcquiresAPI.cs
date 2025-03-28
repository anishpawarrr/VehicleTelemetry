using System;
using Microsoft.AspNetCore.Mvc;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Services.Acquisition;

namespace Telemetry.APIs.V1;

public static class AcquiresAPI
{
    public static WebApplication MapAcquiresEv (this WebApplication app){

        var acquireEv = app.MapGroup("v1/ev/transmit");

        acquireEv.MapPost("", async ([FromBody] TelemetryValuesDTO telemetryValuesDTO, DataBaseContext dbContext) => {
            
            AcquireEV acquireEV = new AcquireEV(dbContext);
            ReturnDTO returnDTO = await acquireEV.AcquireValues(telemetryValuesDTO);
            
            if (!returnDTO.Status){
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                        data = returnDTO.Data
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Created();
        });

        acquireEv.MapPost("batch", async ([FromBody] TelemetryValuesBatchDTO telemetryValuesBatchDTO, DataBaseContext dbContext) => {
            AcquireEV acquireEV = new AcquireEV(dbContext);
            ReturnDTO returnDTO = await acquireEV.AcquireValuesInBatch(telemetryValuesBatchDTO);
            
            if (!returnDTO.Status){
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                        data = returnDTO.Data
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Created();
        });

        acquireEv.MapGet("timestamp", () => {
            return Results.Ok(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        });

        return app;
    }
}
