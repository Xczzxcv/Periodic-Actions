using System;
using System.Collections.Generic;
using Actors.Stats;
using JetBrains.Annotations;
using UniRx;

namespace Actors
{
internal class ActorStatsCollection : Dictionary<ActorStat, ReactiveProperty<double>>, IStatsProvider
{
    public ActorStatsCollection([CanBeNull] IDictionary<ActorStat, double> initValues = null)
    {
        var statValues = Enum.GetValues(typeof(ActorStat));
        foreach (ActorStat stat in statValues)
        {
            double initValue = default;
            initValues?.TryGetValue(stat, out initValue);
            Add(stat, new ReactiveProperty<double>(initValue));
        }
    }

    public double GetStat(ActorStat stat) => Get(stat).Value;

    public IReadOnlyReactiveProperty<double> Get(ActorStat stat) => this[stat];
}
}