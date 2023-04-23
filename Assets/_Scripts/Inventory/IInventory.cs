using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;

namespace Inventory
{
internal interface IInventory
{
    IEnumerable<IActorItem> Items { get; }
    IEnumerable<IActorItem> CellsContent { get; }
    int EmptyCellsCount { get; }
    
    bool TryPutInto(IActorItem item);
    [CanBeNull] IActorItem TakeAway(IActorItem item);
    IObservable<IActorItem> ObservableAdd();
    IObservable<IActorItem> ObservableRemove();
}
}