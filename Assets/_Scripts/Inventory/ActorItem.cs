using Actors;
using Actors.Stats;
using UniRx;

namespace Inventory
{
internal class ActorItem : IActorItem
{
    public string Id => _config.Id;
    public StatsShiftConfig Stats => _config.Stats;

    private readonly ActorItemConfig _config;
    private readonly ActorStatsCollection _stats;

    public ActorItem(ActorItemConfig config)
    {
        _config = config;
        _stats = new ActorStatsCollection(_config.Stats.Shifts);
    }

    public double GetStat(ActorStat stat) => _stats.GetStat(stat);

    public IReadOnlyReactiveProperty<double> Get(ActorStat stat) => _stats.Get(stat);
}
}