using System.Collections.Generic;
using System.Linq;
using Spells;
using UniRx;
using UnityEngine;

namespace Actors
{
internal class SpellsActorManager : ActorManager
{
    public IReadOnlyReactiveProperty<(ISpell Spell, SpellCastInfo CastInfo)> CastingSpell => _castingSpell;
    public IReadOnlyDictionary<string, ISpell> Spells => _spells;

    private readonly TimelineManager _timelineManager;
    private readonly SpellsFactory _spellsFactory;
    private readonly Dictionary<string, ISpell> _spells = new();
    private readonly ReactiveProperty<(ISpell Spell, SpellCastInfo CastInfo)> _castingSpell = new();

    public SpellsActorManager(TimelineManager timelineManager, SpellsFactory spellsFactory, Actor self) 
        : base(self)
    {
        _timelineManager = timelineManager;
        _spellsFactory = spellsFactory;
    }

    public override void Init(Actor.Config config)
    {
        Debug.Assert(config.SpellIds.Length > 0, $"No spells for '{config.Name}'");
        foreach (var spellId in config.SpellIds)
        {
            var spell = _spellsFactory.BuildSpell(spellId);
            AddSpell(spell);
        }
    }
    
    public bool CanStartSpellCast()
    {
        return Self.IsAlive && _castingSpell.Value.Spell == null;
    }

    public bool CanMainCastSpell()
    {
        return Self.IsAlive;
    }

    public void CastSpell(ActorSpellCastChoice spellCastChoice)
    {
        Debug.Assert(spellCastChoice.CastInfo.Caster == Self, 
            $"Invalid caster in spell cast choice {spellCastChoice} != {Self}");
        Debug.Assert(_spells.ContainsKey(spellCastChoice.SpellId),
            $"Nonexistent spellId in spell cast choice {spellCastChoice} != {string.Join(", ", _spells.Keys)}");
        Debug.Assert(CanStartSpellCast(), $"Can't start spell cast {spellCastChoice}");

        if (!_spells.TryGetValue(spellCastChoice.SpellId, out var spell))
        {
            return;
        }

        _timelineManager.AddSpellCastRequest(spell, spellCastChoice.CastInfo);
        _castingSpell.Value = (spell, spellCastChoice.CastInfo);
    }

    public void ProcessCastingEnded()
    {
        _castingSpell.Value = default;
    }

    private void AddSpell(ISpell spell)
    {
        _spells.Add(spell.Id, spell);
    }

    private void RemoveSpell(ISpell spell)
    {
        _spells.Remove(spell.Id);
    }

    public bool HasSpell(string spellId)
    {
        return _spells.ContainsKey(spellId);
    }
    
    #region Debug

    public ISpell GetRandomSpell() => _spells.Values.ElementAt(Random.Range(0, _spells.Count));

    #endregion

    public override void Dispose()
    {
        foreach (var spell in _spells.Values)
        {
            spell?.Dispose();
        }

        base.Dispose();
    }
}
}