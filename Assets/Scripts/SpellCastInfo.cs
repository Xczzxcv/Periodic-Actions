using System;

public struct SpellCastInfo : IEquatable<SpellCastInfo>
{
    public double PreCastTime;
    public ISpellCaster Caster;
    public ISpellTarget Target;
    
    public override string ToString()
    {
        return $"[SCI] PreTime: {PreCastTime}, caster: {Caster}, target: {Target}";
    }

    public bool Equals(SpellCastInfo other)
    {
        return base.Equals(other);
    }
}