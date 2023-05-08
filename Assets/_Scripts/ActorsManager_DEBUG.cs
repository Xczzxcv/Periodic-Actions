using System.Linq;
using Actors;
using UnityEngine;

internal partial class ActorsManager
{
    private string _inventorySlotIndex = "InventorySlotIndex";
    private string _actorName = "ActorName";

    private void OnGUI()
    {
        var textAreaSize = new Vector2(200, 50);
        const int indent = 20;
        var posX = Screen.width - textAreaSize.x - indent;
        var startPosY = Screen.height - (textAreaSize.y + indent) * 4;
        _inventorySlotIndex = GUI.TextField(
            new Rect(new Vector2(posX, startPosY), textAreaSize),
            _inventorySlotIndex
        );

        _actorName = GUI.TextField(
            new Rect(new Vector2(posX, startPosY + (textAreaSize.y + indent)), textAreaSize),
            _actorName
        );

        if (GUI.Button(
                new Rect(new Vector2(posX, startPosY + (textAreaSize.y + indent) * 2), textAreaSize),
                "Move item to actor"
            ))
        {
            ProcessItemToActorBtnClick();
            return;
        }

        if (GUI.Button(
                new Rect(new Vector2(posX, startPosY + (textAreaSize.y + indent) * 3), textAreaSize),
                "Move item from actor"
            ))
        {
            ProcessItemFromActorBtnClick();
            return;
        }
    }

    private void ProcessItemToActorBtnClick()
    {
        if (!int.TryParse(_inventorySlotIndex, out var itemIndex))
        {
            Debug.LogError($"Invalid inventory slot index '{_inventorySlotIndex}'");
            return;
        }

        var item = _playerInventory.CellsContent.ElementAt(itemIndex);
        var resultActor = Enumerable.Empty<IActor>()
            .Concat(_playerTeam.Actors)
            .Concat(_enemyTeam.Actors)
            .FirstOrDefault(actor => actor.Name == _actorName);

        if (resultActor == null)
        {
            Debug.LogError($"Has no actor with name {_actorName}");
            return;
        }

        if (resultActor.Inventory.EmptyCellsCount <= 0)
        {
            Debug.LogError($"{resultActor} has no empty slots in inventory");
            return;
        }

        var takenItem = _playerInventory.TakeAway(item);
        if (!resultActor.Inventory.TryPutInto(takenItem))
        {
            Debug.LogError($"We somehow can't put {takenItem} into {resultActor} inventory");
        }
    }

    private void ProcessItemFromActorBtnClick()
    {
        var resultActor = Enumerable.Empty<IActor>()
            .Concat(_playerTeam.Actors)
            .Concat(_enemyTeam.Actors)
            .FirstOrDefault(actor => actor.Name == _actorName);

        if (resultActor == null)
        {
            Debug.LogError($"Has no actor with name {_actorName}");
            return;
        }

        var firstActorItem = resultActor.Inventory.CellsContent.First();
        if (firstActorItem == null)
        {
            Debug.LogError($"{resultActor} has no item in first slot");
            return;
        }

        var takenItem = resultActor.Inventory.TakeAway(firstActorItem);
        if (!_playerInventory.TryPutInto(takenItem))
        {
            Debug.LogError($"We somehow can't put {takenItem} into player inventory inventory");
        }
    }
}