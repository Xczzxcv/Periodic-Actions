namespace Actors.Stats
{
internal interface IStatsShiftFactory
{
    IStatsShift Create(StatsShiftFactory.Args args);
}
}