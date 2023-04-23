using System.Collections.Generic;

namespace Actors.Stats
{
public class TempCountStatsShiftConfig : StatsShiftConfig
{
    public int SpellUsagesCounter;

    public override Dictionary<string, object> GetProperties()
    {
        var properties =  base.GetProperties();
        properties.Add("spell use counter", SpellUsagesCounter);

        return properties;
    }
}
}