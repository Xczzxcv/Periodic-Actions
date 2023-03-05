using System;

namespace SpellConfigs
{
[Serializable]
public abstract class SpellConfigBase
{
    public string Id;
    public int Damage;
    public int Armor;
    public int Duration;

    public override string ToString() => $"Spell config ({Id})";
}
}