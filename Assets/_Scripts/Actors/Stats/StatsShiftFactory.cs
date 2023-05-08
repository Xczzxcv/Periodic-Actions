namespace Actors.Stats
{
internal class StatsShiftFactory : IStatsShiftFactory
{
    public struct Args
    {
        public StatsShiftConfig statsShiftConfig;
        public IActor owner;
        public string SourceId;
    }

    public IStatsShift Create(Args args)
    {
        return args.statsShiftConfig switch
        {
            TempCountStatsShiftConfig tempCountConfig => new TempCountStatsShift(tempCountConfig, args.owner, args.SourceId),
            ItemStatsShiftConfig persistentConfig => new ItemStatsShift(persistentConfig, args.owner, args.SourceId),
            _ => throw ThrowHelper.WrongConfigTypeInFactory<StatsShiftConfig, IStatsShift>(args.statsShiftConfig)
        };
    }
}
}