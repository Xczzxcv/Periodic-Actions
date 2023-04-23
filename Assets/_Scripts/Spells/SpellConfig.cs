using System;
using Spells.Aspects;
using UnityEngine;

namespace Spells
{
[CreateAssetMenu(menuName = "Configs/Spell", fileName = "SpellConfig", order = 0)]
[Serializable]
internal class SpellConfig : ScriptableObject
{
    public string Id;
    public int Delay;
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
    public SpellAspectConfig[] Aspects;

    public override string ToString() => $"Spell config ({Id})";
}
}