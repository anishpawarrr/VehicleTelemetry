using System;
using Telemetry.DTOs;

namespace Telemetry.Services.Setup;

public interface ISetup<TCreate, TRetrieve, TUpdate, TDelete>
{
    Task<ReturnDTO> Create(TCreate create);
    Task<ReturnDTO> Retrieve(TRetrieve retrieve);
    Task<ReturnDTO> Update(TUpdate update);
    Task<ReturnDTO> Delete(TDelete delete);
}
