using Actors;

namespace Inventory
{
internal class ActorInventory : Inventory
{
    private readonly IActor _owner;
    
    private const int ActorInventorySize = 1;
    
    public ActorInventory(IActor owner) : base(ActorInventorySize)
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