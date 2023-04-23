using UniRx;
using UnityEngine;

namespace Inventory
{
internal class InventoryCell
{
    public IReadOnlyReactiveProperty<IActorItem> Item => _item;
    private readonly ReactiveProperty<IActorItem> _item; 

    public bool IsEmpty => Item.Value == null;

    public InventoryCell(IActorItem actorItem = null)
    {
        _item = new ReactiveProperty<IActorItem>(actorItem);
    }

    public void PutInto(IActorItem item)
    {
        Debug.Assert(IsEmpty, $"Cell should be emptied before putting {item} (now has {Item.Value})");
       
        _item.Value = item;
    }

    public IActorItem TakeAway()
    {
        Debug.Assert(!IsEmpty, $"Cell should contain something before trying to take smh away");
        
        var resultItem = _item.Value;
        _item.Value = null;
        return resultItem;
    }
}
}