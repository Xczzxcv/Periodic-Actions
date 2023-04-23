using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Inventory
{
internal abstract class Inventory : IInventory
{
    public IEnumerable<IActorItem> Items => Cells
        .Where(cell => !cell.IsEmpty)
        .Select(cell => cell.Item.Value);

    public IEnumerable<IActorItem> CellsContent => Cells
        .Select(cell => cell.Item.Value);

    public int EmptyCellsCount => Cells.Count(cell => cell.IsEmpty);

    protected readonly InventoryCell[] Cells;
    private readonly Subject<IActorItem> collectionAdd = new();
    private readonly Subject<IActorItem> collectionRemove = new();

    protected Inventory(int size)
    {
        Cells = new InventoryCell[size];
        for (var index = 0; index < Cells.Length; index++)
        {
            Cells[index] = new InventoryCell();
        }
    }

    public virtual bool TryPutInto(IActorItem item)
    {
        foreach (var cell in Cells)
        {
            if (!cell.IsEmpty)
            {
                continue;
            }

            cell.PutInto(item);
            collectionAdd.OnNext(item);
            return true;
        }

        return false;
    }

    public virtual IActorItem TakeAway(IActorItem item)
    {
        foreach (var cell in Cells)
        {
            if (cell.Item.Value != item)
            {
                continue;
            }

            var resultItem = cell.TakeAway();
            collectionRemove.OnNext(resultItem);
            return resultItem;
        }

        return null;
    }

    public IObservable<IActorItem> ObservableAdd() => collectionAdd;

    public IObservable<IActorItem> ObservableRemove() => collectionRemove;
}
}