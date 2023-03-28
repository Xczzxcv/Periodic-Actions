using System.Collections.Generic;
using UniRx;

namespace Actors
{
internal class ActorStatsCollection : Dictionary<string, ReactiveProperty<double>>
{ }
}