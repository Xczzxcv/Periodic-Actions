using System.Collections.Generic;
using System.Linq;
using Spells;
using UniRx;
using UnityEngine;

internal class TimelineManager
{
    internal enum UpdateResult
    {
        NoSpellsProcessed,
        SpellProcessedButFailedToBeCasted,
        SpellProcessedAndCasted,
    }

    public double CurrentTime => _timeManager.CurrentTime.Value;
    public bool IsPaused => _timeManager.IsPaused.Value;

    public IReadOnlyReactiveProperty<(ISpell Spell, SpellCastInfo CastInfo)> InitCastedSpellInfo 
        => _initCastedSpellInfo;
    public IReadOnlyReactiveProperty<DeferredSpellCastInfo> MainCastedSpellInfo 
        => _mainCastedSpellInfo;
    public IReadOnlyReactiveProperty<DeferredSpellCastInfo> PostCastedSpellInfo 
        => _postCastedSpellInfo;

    private readonly ReactiveProperty<(ISpell Spell, SpellCastInfo CastInfo)> _initCastedSpellInfo = new ();
    private readonly ReactiveProperty<DeferredSpellCastInfo> _mainCastedSpellInfo = new ();
    private readonly ReactiveProperty<DeferredSpellCastInfo> _postCastedSpellInfo = new ();
    private readonly List<DeferredSpellCastInfo> _spellsToCast = new ();
    private readonly TimeManager _timeManager;

    public TimelineManager(TimeManager timeManager)
    {
        _timeManager = timeManager;
    }

    public void AddSpellCastRequest(ISpell spell, SpellCastInfo castInfo)
    {
        InitialCastSpell(spell, castInfo);
        var castTime = castInfo.InitialCastTime + spell.BaseConfig.Delay;
        _spellsToCast.Add(new DeferredSpellCastInfo
        (
            castTime,
            spell,
            castInfo,
            castInfo.Caster.IsPlayerUnit
        ));
        _spellsToCast.Sort(DeferredSpellCastInfo.TimeComparison);
    }

    public UpdateResult Update()
    {
        if (IsPaused)
        {
            return UpdateResult.NoSpellsProcessed;
        }
        
        if (!_spellsToCast.Any())
        {
            return UpdateResult.NoSpellsProcessed;
        }

        var deferredCastInfo = _spellsToCast.Last();
        if (_timeManager.CurrentTime.Value < deferredCastInfo.CastTime)
        {
            return UpdateResult.NoSpellsProcessed;
        }

        _spellsToCast.RemoveAt(_spellsToCast.Count - 1);

        if (!deferredCastInfo.CastInfo.Caster.Spells.CanMainCastSpell())
        {
            PostCastSpell(deferredCastInfo);
            return UpdateResult.SpellProcessedButFailedToBeCasted;
        }

        MainCastSpell(deferredCastInfo);
        PostCastSpell(deferredCastInfo);
        return UpdateResult.SpellProcessedAndCasted;
    }

    private void InitialCastSpell(ISpell spell, SpellCastInfo castInfo)
    {
        Debug.Log($"[{CurrentTime}] {spell} being INIT casted " +
                  $"by {castInfo.Caster} on {castInfo.Target}");
        spell.InitialCast(castInfo);
        _initCastedSpellInfo.Value = (Spell: spell, CastInfo: castInfo);
    }

    private void MainCastSpell(DeferredSpellCastInfo deferredCastInfo)
    {
        Debug.Log($"[{CurrentTime}] {deferredCastInfo.Spell} being MAIN casted " +
                  $"by {deferredCastInfo.CastInfo.Caster} on {deferredCastInfo.CastInfo.Target}");
        deferredCastInfo.Spell.MainCast(deferredCastInfo.CastInfo);
        _mainCastedSpellInfo.Value = deferredCastInfo;
    }

    private void PostCastSpell(DeferredSpellCastInfo deferredCastInfo)
    {
        Debug.Log($"[{CurrentTime}] {deferredCastInfo.Spell} being POST casted " +
                  $"by {deferredCastInfo.CastInfo.Caster} on {deferredCastInfo.CastInfo.Target}");
        deferredCastInfo.Spell.PostCast(deferredCastInfo.CastInfo);
        deferredCastInfo.CastInfo.Caster.Spells.ProcessCastingEnded();
        _postCastedSpellInfo.Value = deferredCastInfo;
    }

    public void Pause()
    {
        _timeManager.Pause();
    }

    public void Resume()
    {
        _timeManager.Resume();
    }
}