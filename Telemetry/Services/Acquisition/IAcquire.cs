using System;
using Telemetry.Data;
using Telemetry.DTOs;

namespace Telemetry.Services.Acquisition;

public interface IAcquire
{
    Task<ReturnDTO> AcquireValues(TelemetryValuesDTO telemetryValuesDTO);
    Task<ReturnDTO> AcquireValuesInBatch(TelemetryValuesBatchDTO telemetryValuesBatchDTO);
    Task<ReturnDTO> LapUpdate(LapCountUpdateDTO lapCountUpdateDTO);
    Task<ReturnDTO> TurnMetrics(TurnMetricsDTO turnMetricsDTO);
}
