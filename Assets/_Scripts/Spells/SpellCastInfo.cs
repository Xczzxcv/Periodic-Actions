using System;
using Actors;
using JetBrains.Annotations;

namespace Spells
{
internal readonly struct SpellCastInfo : IEquatable<SpellCastInfo>
{
    public readonly int InitialCastTime;
    public readonly IActor Caster;
    [CanBeNull] public readonly IActor Target;

    public SpellCastInfo(
        int initialCastTime,
        IActor caster,
        [CanBeNull] IActor target
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