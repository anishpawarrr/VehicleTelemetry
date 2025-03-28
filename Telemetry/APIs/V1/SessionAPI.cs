using System;
using Microsoft.AspNetCore.Mvc;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Services.Setup;

namespace Telemetry.APIs.V1;

public static class SessionAPI
{
    public static WebApplication MapSession(this WebApplication app)
    {
        var session = app.MapGroup("v1/session");

        session.MapPost("", async ([FromBody] AddSessionDTO addSessionDTO, DataBaseContext dbContext) => {
            
            SessionSetup sessionService = new SessionSetup(dbContext);
            ReturnDTO returnDTO = await sessionService.Create(addSessionDTO);

            if (!returnDTO.Status)
            {
                return Results.Json(
                    new {
                        message = returnDTO.Message,
                    },
                    statusCode: returnDTO.Data is int ? (int)returnDTO.Data : StatusCodes.Status500InternalServerError
                );
            }

            return Results.Created(uri: "v1/session", value: returnDTO.Data);
        });

        session.MapGet("{Id}", async ([FromRoute] int Id, DataBaseContext dbContext) => {
            
            SessionSetup sessionService = new SessionSetup(dbContext);
            ReturnDTO returnDTO = await sessionService.Retrieve(Id);

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

        session.MapPut("", async ([FromBody] UpdateSessionDTO updateSessionDTO, DataBaseContext dbContext) => {
            
            SessionSetup sessionService = new SessionSetup(dbContext);
            ReturnDTO returnDTO = await sessionService.Update(updateSessionDTO);

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

        session.MapDelete("{Id}", async ([FromRoute] int Id, DataBaseContext dbContext) => {
            
            SessionSetup sessionService = new SessionSetup(dbContext);
            ReturnDTO returnDTO = await sessionService.Delete(Id);

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