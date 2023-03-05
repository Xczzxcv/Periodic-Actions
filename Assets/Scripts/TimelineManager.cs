using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

internal class TimelineManager
{
    public double CurrentTime => _timeManager.CurrentTime.Value;
    public IReadOnlyReactiveProperty<DeferredSpellCastInfo> CastedSpellInfo => _castedSpellInfo;
    private readonly ReactiveProperty<DeferredSpellCastInfo> _castedSpellInfo = new ();

    private readonly List<DeferredSpellCastInfo> _spellsToCast = new ();
    private readonly TimeManager _timeManager;

    public TimelineManager(TimeManager timeManager)
    {
        _timeManager = timeManager;
    }
    
    public void AddSpellCastRequest(ISpell spell, SpellCastInfo castInfo)
    {
        PreCastSpell(spell, castInfo);
        var castTime = castInfo.PreCastTime + spell.Duration;
        _spellsToCast.Add(new DeferredSpellCastInfo
        (
            castTime,
            spell,
            castInfo,
            castInfo.Caster.IsPlayerUnit
        ));
        _spellsToCast.Sort(DeferredSpellCastInfo.TimeComparison);
    }

    public bool Update()
    {
        if (!_spellsToCast.Any())
        {
            return false;
        }

        var deferredCastInfo = _spellsToCast.Last();
        if (_timeManager.CurrentTime.Value < deferredCastInfo.CastTime)
        {
            return false;
        }

        _spellsToCast.RemoveAt(_spellsToCast.Count - 1);

        CastSpell(deferredCastInfo);
        return true;
    }

    private void PreCastSpell(ISpell spell, SpellCastInfo castInfo)
    {
        Debug.Log($"[{CurrentTime}] {spell} being PRE casted " +
                  $"by {castInfo.Caster} on {castInfo.Target}");
        spell.PreCast(castInfo);
    }

    private void CastSpell(DeferredSpellCastInfo deferredCastInfo)
    {
        Debug.Log($"[{CurrentTime}] {deferredCastInfo.Spell} being casted " +
                  $"by {deferredCastInfo.CastInfo.Caster} on {deferredCastInfo.CastInfo.Target}");
        deferredCastInfo.Spell.Cast(deferredCastInfo.CastInfo);
        _castedSpellInfo.Value = deferredCastInfo;
    }
}