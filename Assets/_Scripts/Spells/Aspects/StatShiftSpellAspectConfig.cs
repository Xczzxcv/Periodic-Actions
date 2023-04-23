using Actors.Stats;
using UnityEngine;

namespace Spells.Aspects
{
public abstract class StatShiftSpellAspectConfig : SpellAspectConfig
{
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
    public StatsShiftConfig Stats;
}
}