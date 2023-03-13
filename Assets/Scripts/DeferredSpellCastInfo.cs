using System;
using Spells;

internal readonly struct DeferredSpellCastInfo
{
    public readonly double CastTime;
    public readonly ISpell Spell;
    public readonly SpellCastInfo CastInfo;
    public readonly bool IsOrderFromPlayer;
    private readonly long _orderNumber;
    
    private static long _globalOrderNumber;

    public DeferredSpellCastInfo(
        double castTime,
        ISpell spell,
        SpellCastInfo castInfo,
        bool isOrderFromPlayer
    )
    {
        CastTime = castTime;
            Spell = spell;
        CastInfo = castInfo;
        IsOrderFromPlayer = isOrderFromPlayer;
        _orderNumber = _globalOrderNumber++;
    }
    
    public override string ToString()
    {
        return $"[DSCI] Time: {CastTime}, {Spell}, {CastInfo}";
    }

    public static int TimeComparison(DeferredSpellCastInfo x, DeferredSpellCastInfo y)
    {
        // descending
        var timeComparison = Math.Sign(y.CastTime - x.CastTime);
        if (timeComparison != 0)
        {
            return timeComparison;
        }

        if (x.IsOrderFromPlayer == y.IsOrderFromPlayer)
        {
            // queue imitation
            return Math.Sign(y._orderNumber - x._orderNumber);
        }

        // descending
        return x.IsOrderFromPlayer
            ? 1
            : -1;
    }
}