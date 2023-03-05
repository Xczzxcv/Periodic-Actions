using System;

readonly struct SpellCastInfo : IEquatable<SpellCastInfo>
{
    public readonly double PreCastTime;
    public readonly ISpellCaster Caster;
    public readonly ISpellTarget Target;

    public SpellCastInfo(
        double preCastTime,
        ISpellCaster caster,
        ISpellTarget target
    )
    {
        PreCastTime = preCastTime;
        Caster = caster;
        Target = target;
    }

    public override string ToString()
    {
        return $"[SCI] PreTime: {PreCastTime}, caster: {Caster}, target: {Target}";
    }

    public bool Equals(SpellCastInfo other)
    {
        return base.Equals(other);
    }
}