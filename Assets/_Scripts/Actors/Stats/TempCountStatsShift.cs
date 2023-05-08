using System;
using Spells;
using UniRx;
using UnityEngine;

namespace Actors.Stats
{
internal class TempCountStatsShift : StatsShift<TempCountStatsShiftConfig>
{
    private IDisposable _subscription;
    private int _counter;

    public TempCountStatsShift(TempCountStatsShiftConfig config, IActor owner, string sourceId) 
        : base(config, owner, sourceId)
    {
        _counter = -1; // for increase when Subscribe called
    }

    protected override void SubscribeToStatRemovalEvent()
    {
        Debug.Assert(_subscription == null);
        
        _subscription = Owner.Spells.CastingSpell.Subscribe(OnNewCastingSpell);
    }

    private void OnNewCastingSpell((ISpell Spell, SpellCastInfo CastInfo) value)
    {
        if (value.Spell != null)
        {
            return;
        }

        _counter++;
        if (_counter >= Config.SpellUsagesCounter)
        {
            CallStatsShiftEnded();
        }
    }

    protected override void UnSubscribeFromStatRemovalEvent()
    {
        Debug.Assert(_subscription != null);

        _subscription.Dispose();
        _subscription = null;
    }
}
}