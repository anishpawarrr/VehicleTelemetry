using Telemetry.Data;
using Telemetry.APIs.V1;
using Telemetry.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DB");
builder.Services.AddSqlite<DataBaseContext>(connectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Telemetry API",
        Version = "v1",
        Description = "An API for managing telemetry data",
        Contact = new OpenApiContact
        {
            Name = "Anish Pawar",
            Email = "anishpurupawar@gmail.com"
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Telemetry API v1");
        options.RoutePrefix = "swagger";
    });
}


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
