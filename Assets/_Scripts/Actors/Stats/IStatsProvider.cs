using UniRx;

namespace Actors.Stats
{
internal interface IStatsProvider
{
    double GetStat(ActorStat stat);
    IReadOnlyReactiveProperty<double> Get(ActorStat stat);
}
}