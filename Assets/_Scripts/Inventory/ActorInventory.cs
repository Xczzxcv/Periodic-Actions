using Actors;

namespace Inventory
{
internal class ActorInventory : Inventory
{
    private readonly Actor _owner;
    
    private const int ActorInventorySize = 1;
    
    public ActorInventory(Actor owner) : base(ActorInventorySize)
    {
        _owner = owner;
    }

    public override bool TryPutInto(IActorItem item)
    {
        var result = base.TryPutInto(item);
        if (result)
        {
            _owner.Stats.AddStatShift(item.Stats, item.Id);
        }

        return result;
    }
}
}