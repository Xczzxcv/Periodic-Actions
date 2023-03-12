using System;

namespace Spells
{
internal readonly struct SpellCastInfo : IEquatable<SpellCastInfo>
{
    public readonly double InitialCastTime;
    public readonly Actor Caster;
    public readonly Actor Target;

    public SpellCastInfo(
        double initialCastTime,
        Actor caster,
        Actor target
    )
    {
        InitialCastTime = initialCastTime;
        Caster = caster;
        Target = target;
    }

    public override string ToString()
    {
        return $"[SCI] InitTime: {InitialCastTime}, caster: {Caster}, target: {Target}";
    }

    public bool Equals(SpellCastInfo other)
    {
        return base.Equals(other);
    }
}
}