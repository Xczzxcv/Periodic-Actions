using System;
using Actors.Stats;
using UnityEngine;

namespace Inventory
{
[CreateAssetMenu(menuName = "Configs/Actor Item", fileName = "ActorItemConfig", order = 0)]
[Serializable]
internal class ActorItemConfig : ScriptableObject
{
    public string Id;
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
    public StatsShiftConfig Stats;
}
}