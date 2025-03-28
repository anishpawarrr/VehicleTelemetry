using System;
using Microsoft.AspNetCore.Mvc;
using Telemetry.Data;
using Telemetry.DTOs;

namespace Telemetry.APIs.V1;

public static class StreamAPI
{
    public static WebApplication MapStream (this WebApplication app){

        var stream = app.MapGroup("v1/stream");

        stream.MapGet("{sessionId}/{latestPacketId?}", async ([FromRoute] int sessionId, [FromRoute] int? latestPacketId, DataBaseContext dbContext) => {

            latestPacketId ??= 0;

            Services.Presentation.Stream streamService = new(dbContext);
            ReturnDTO returnDTO = await streamService.StreamData(sessionId, latestPacketId.Value);
            
            if (!returnDTO.Status){
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Ok(returnDTO.Data);
        });
        
        stream.MapGet("{sessionId}/lap/{lapNumber}", async ([FromRoute] int sessionId, [FromRoute] int lapNumber, DataBaseContext dbContext) => {
            
            Services.Presentation.Stream streamService = new(dbContext);
            ReturnDTO returnDTO = await streamService.StreamLapData(sessionId, lapNumber);

            if (!returnDTO.Status){
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }
            return Results.Ok(returnDTO.Data);
        });

        stream.MapGet("{sessionId}/laps", async ([FromRoute] int sessionId, DataBaseContext dbContext) => {
            
            Services.Presentation.Stream streamService = new(dbContext);
            ReturnDTO returnDTO = await streamService.GetLapNumbers(sessionId);

            if (!returnDTO.Status){
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }
            return Results.Ok(returnDTO.Data);
        });

        stream.MapGet("{sessionId}/laptimes", async ([FromRoute] int sessionId, DataBaseContext dbContext) => {
            
            Services.Presentation.Stream streamService = new(dbContext);
            ReturnDTO returnDTO = await streamService.GetLapTimes(sessionId);

            if (!returnDTO.Status){
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
