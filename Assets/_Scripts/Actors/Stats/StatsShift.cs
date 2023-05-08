using System;
using UniRx;

namespace Actors.Stats
{
internal abstract class StatsShift<TConfig> : IStatsShift
where TConfig : StatsShiftConfig
{
    public string SourceId { get; }
    public event Action<IStatsShift> StatShiftEnded;

    protected readonly TConfig Config;
    protected readonly IActor Owner;
    protected readonly ActorStatsCollection Stats;

    protected StatsShift(TConfig config, IActor owner, string sourceId)
    {
        Config = config;
        Owner = owner;
        SourceId = sourceId;

        Stats = new ActorStatsCollection(Config.Shifts);
    }

    public void Init()
    {
        SubscribeToStatRemovalEvent();
    }

    protected abstract void SubscribeToStatRemovalEvent();
    protected abstract void UnSubscribeFromStatRemovalEvent();

    protected void CallStatsShiftEnded() => StatShiftEnded?.Invoke(this);

    public double GetStat(ActorStat stat)
    {
        return Stats.GetStat(stat);
    }

    public IReadOnlyReactiveProperty<double> Get(ActorStat stat) => Stats.Get(stat);

    public virtual void Dispose()
    {
        UnSubscribeFromStatRemovalEvent();
    }
}
}