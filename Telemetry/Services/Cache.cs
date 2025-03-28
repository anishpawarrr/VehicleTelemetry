using System;
using System.Collections.Concurrent;
using Telemetry.DTOs;

namespace Telemetry.Services;

public static class Cache
{
    private static ConcurrentDictionary<int, CacheDTO> _CacheDictionary = new ConcurrentDictionary<int, CacheDTO>();

    public static void Set(int SessionId, CacheDTO cacheDTO)
    {
        _CacheDictionary[SessionId] = cacheDTO;
    }

    public static CacheDTO Get(int SessionId)
    {
        if(_CacheDictionary.ContainsKey(SessionId))
        {
            return _CacheDictionary[SessionId];
        }else{
            CacheDTO cacheDTO = new CacheDTO{
                Speed = 0,
                Timestamp = 0,
                LapNumber = 0,
                BatteryCapacity = 60,
                CurrentBatteryPercentage = 100
            };
            Set(SessionId, cacheDTO);
            return cacheDTO;
        }
        throw new Exception("SessionId (Key) not found in cache");
    }

    public static void GarbageCollector()
    {
        foreach (int sessionId in _CacheDictionary.Keys.ToList())
        {
            if (_CacheDictionary[sessionId].ExpiresAt < DateTime.Now)
            {
                _CacheDictionary.TryRemove(sessionId, out _);
            }
        }
    }

}
