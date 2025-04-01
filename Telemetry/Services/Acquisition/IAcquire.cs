using System;
using Telemetry.Data;
using Telemetry.DTOs;

namespace Telemetry.Services.Acquisition;

/// <summary>
/// Interface for acquiring telemetry data.
/// </summary>
public interface IAcquire
{
    /// <summary>
    /// Acquires telemetry values from the provided DTO.
    /// This method is responsible for processing the telemetry data and storing it in the database.
    /// </summary>
    /// <param name="telemetryValuesDTO">Values of the instance</param>
    /// <returns>ReturnDTO {status: bool, message: string, data: object}</returns>
    Task<ReturnDTO> AcquireValues(TelemetryValuesDTO telemetryValuesDTO);

    /// <summary>
    /// Acquires telemetry values in batch from the provided DTO.
    /// This method is responsible for processing the telemetry data and storing it in the database.
    /// </summary>
    /// <param name="telemetryValuesBatchDTO">Values in batch of the instance</param>
    /// <returns>ReturnDTO {status: bool, message: string, data: object}</returns>
    Task<ReturnDTO> AcquireValuesInBatch(TelemetryValuesBatchDTO telemetryValuesBatchDTO);

    /// <summary>
    /// Updates the lap count for the telemetry data.
    /// </summary>
    /// <param name="lapCountUpdateDTO">new lap number</param>
    /// <returns>ReturnDTO {status: bool, message: string, data: object}</returns>
    Task<ReturnDTO> LapUpdate(LapCountUpdateDTO lapCountUpdateDTO);
    Task<ReturnDTO> TurnMetrics(TurnMetricsDTO turnMetricsDTO);
}
