using System;
using System.Collections.Generic;
using System.Linq;
using Actors.Stats;
using JetBrains.Annotations;
using UnityEngine;

namespace Inventory
{
internal class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private ActorItemConfig[] itemConfigs;

    private PlayerInventory _playerInventory;

    public PlayerInventory GetInventory()
    {
        _playerInventory ??= new PlayerInventory(size);
        UpdateInventoryContent();

        return _playerInventory;
    }

    [EditorButton(nameof(UpdateInventoryContent), activityType: ButtonActivityType.OnPlayMode), Hide, SerializeField] private bool btn;
    private void UpdateInventoryContent()
    {
        Debug.Assert(itemConfigs.Length < size);

        int index;
        for (index = 0; index < itemConfigs.Length; index++)
        {
            var itemConfig = itemConfigs[index];
            var item = new ActorItem(itemConfig);
            _playerInventory.TryPutInto(item);
        }
    }

    public static IEnumerable<(ActorStat stat, double value)> GetNonDefaultStats(
        [CanBeNull] IStatsProvider statsProvider)
    {
        if (statsProvider == null)
        {
            yield break;
        }

        var stats = Enum.GetValues(typeof(ActorStat));
        foreach (ActorStat stat in stats)
        {
            var statValue = statsProvider.GetStat(stat);
            if (statValue != default)
            {
                yield return new(stat, statValue);
            }
        }
    }

    private void OnGUI()
    {
        var inventoryItems = _playerInventory.Items.ToArray();
        var itemsCount = inventoryItems.Count();
        var inventoryText = $"INVENTORY ({itemsCount}):\n";
        for (var index = 0; index < inventoryItems.Length; index++)
        {
            var inventoryItem = inventoryItems[index];
            var cellContentName = inventoryItem.Id;
            var cellContentProperties = GetNonDefaultStats(inventoryItem);
            var cellContentPropertiesStr = string.Join('\n',
                cellContentProperties.Select(tuple => $"  * {tuple.stat}: {tuple.value}"));
            var cellContentStr = $"{cellContentName}:\n{cellContentPropertiesStr}";

            var cellText = $"{index}) {cellContentStr}";
            inventoryText += $"{cellText}\n";
        }

        var indent = new Vector2(0, 20);
        var labelSize = new Vector2(220, 200);
        var labelStartPoint = new Vector2(0 + indent.x, Screen.height - indent.y - labelSize.y);
        GUI.Label(new Rect(labelStartPoint, labelSize), inventoryText);
    }
}
}