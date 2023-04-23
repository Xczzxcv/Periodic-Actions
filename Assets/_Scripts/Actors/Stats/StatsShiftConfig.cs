using System;
using System.Collections.Generic;
using UnityEngine;

namespace Actors.Stats
{
[Serializable]
public abstract class StatsShiftConfig : IHaveProperties
{
    public SerializedDictionary<ActorStat, double> Shifts;
    
    public virtual Dictionary<string, object> GetProperties()
    {
        var properties = new Dictionary<string, object>();
        foreach (var (stat, shift) in Shifts)
        {
            properties.Add($"stat '{stat}' shift", shift);
        }

        return properties;
    }
}
}