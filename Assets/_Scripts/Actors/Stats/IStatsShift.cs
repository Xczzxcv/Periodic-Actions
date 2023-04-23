using System;

namespace Actors.Stats
{
internal interface IStatsShift : IStatsProvider, IDisposable
{
    string SourceId { get; }
    event Action<IStatsShift> StatShiftEnded;
    void Init();
}
}