using System;
using Microsoft.AspNetCore.Mvc;
using Telemetry.Data;
using Telemetry.DTOs;

namespace Telemetry.APIs.V1;

/// <summary>
/// Controller class for managing stream-related operations.
/// </summary>
public static class StreamAPI
{
    /// <summary>
    /// Maps the stream-related endpoints to the application.
    /// </summary>
    /// <param name="app"> The WebApplication </param>
    /// <returns> The web application with mapped Stream endpoints. </returns>
    public static WebApplication MapStream (this WebApplication app){

        var stream = app.MapGroup("v1/stream");

        // Stream data
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

        // Stream lap data      
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

        // Shows list of laps
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

        // Shows list of lap times
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
