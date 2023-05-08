using System;
using Inventory;
using UniRx;

namespace Actors.Stats
{
internal class ItemStatsShift : StatsShift<ItemStatsShiftConfig>
{
    private IDisposable _subscription;

    public ItemStatsShift(ItemStatsShiftConfig config, IActor owner, string sourceId) 
        : base(config, owner, sourceId)
    { }

    protected override void SubscribeToStatRemovalEvent()
    {
        _subscription = Owner.Inventory.ObservableRemove().Subscribe(OnNextItemRemoved);
    }

    private void OnNextItemRemoved(IActorItem removedItem)
    {
        var isRemovedSourceItem = removedItem.Id == SourceId;
        if (!isRemovedSourceItem)
        {
            return;
        }
        
        CallStatsShiftEnded();
    }

    protected override void UnSubscribeFromStatRemovalEvent()
    {
        _subscription?.Dispose();
        _subscription = null;
    }
}
}