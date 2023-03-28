using System;
using UnityEngine.Serialization;

namespace SpellConfigs
{
[Serializable]
public abstract class SpellConfigBase
{
    public string Id;
    public int Damage;
    public int Armor;
    [FormerlySerializedAs("Duration")]
    public int Delay;

    public override string ToString() => $"Spell config ({Id})";
}
}