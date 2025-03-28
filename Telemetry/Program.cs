using Microsoft.AspNetCore.Http.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Telemetry.Data;
using Telemetry.DTOs;
using Telemetry.Services.Acquisition;
using Telemetry.Services.Setup;
using Telemetry.Services.Presentation;
using Telemetry.APIs.V1;
using Telemetry.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DB");
builder.Services.AddSqlite<DataBaseContext>(connectionString);

var app = builder.Build();


app.MapGet("/", () => "Telemetry API is running!");

// app.MigrateDb();

app.MapVehicle();

app.MapSession();

app.MapAcquiresEv();

app.MapStream();

app.MapPut("garbage-collector", () => {
    Cache.GarbageCollector();
    return Results.NoContent();
});

app.Run();
