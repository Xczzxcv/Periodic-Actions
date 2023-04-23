using Actors.Stats;

namespace Inventory
{
internal interface IActorItem : IStatsProvider
{
    string Id { get; }

    StatsShiftConfig Stats { get; }
}
}