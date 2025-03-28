using System;
using Microsoft.EntityFrameworkCore;
using Telemetry.Entities;

namespace Telemetry.Data;

public class DataBaseContext(DbContextOptions<DataBaseContext> options) : DbContext(options)
{
    public DbSet<Session> Sessions { get; set; }
    public DbSet<TelemetryValue> TelemetryValues { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                Id = 1,
                Name = "Anish",
                Password = "Password"
            }
        );
    }
}