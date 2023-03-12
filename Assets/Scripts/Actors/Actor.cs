using System;
using System.Collections.Generic;
using System.Linq;
using Actors.Ai;
using JetBrains.Annotations;
using Spells;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Actors
{
internal class Actor : IDisposable
{
    [Serializable]
    internal struct Config
    {
        public string Name;
        public float Hp;
        public float Armor;
        public ActorSide Side;
        public string AiConfig;
        public string[] SpellIds;
    }

    public bool IsPlayerUnit => Side.Value == ActorSide.Player;
    public bool IsAlive => !IsDead.Value;

    public IReadOnlyReactiveProperty<float> Hp => _hp;
    public IReadOnlyReactiveProperty<float> Armor => _armor;
    public IReadOnlyReactiveProperty<(ISpell Spell, SpellCastInfo CastInfo)> CastingSpell => _castingSpell;
    public IReadOnlyReactiveProperty<ActorSide> Side => _side;
    public IReadOnlyReactiveProperty<bool> IsDead { get; private set; }
    [CanBeNull] public string Name { get; private set; }
    public IReadOnlyDictionary<string, ISpell> Spells => _spells;

    private readonly TimelineManager _timelineManager;
    private readonly SpellsFactory _spellsFactory;

    private ReactiveProperty<float> _hp;
    private ReactiveProperty<float> _armor;
    private readonly ReactiveProperty<(ISpell Spell, SpellCastInfo CastInfo)> _castingSpell = new();
    private ReactiveProperty<ActorSide> _side;
    private readonly Dictionary<string, ISpell> _spells = new();
    
    private IActorAi _ai;

    public Actor(TimelineManager timelineManager, SpellsFactory spellsFactory)
    {
        _timelineManager = timelineManager;
        _spellsFactory = spellsFactory;
    }

    public void Init(Config config)
    {
        Debug.Assert(config.SpellIds.Length > 0);

        _hp = new (config.Hp);
        _armor = new (config.Armor);
        _side = new (config.Side);
        IsDead = _hp.Select(hp => hp <= 0).ToReactiveProperty();
        _ai = new RandomActorAi(this);
        Name = config.Name;

        foreach (var spellId in config.SpellIds)
        {
            var spell = _spellsFactory.BuildSpell(spellId);
            AddSpell(spell);
        }

        _hp.Subscribe(hp => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(hp)} updated: {hp}"));
        _armor.Subscribe(armor => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(armor)} updated: {armor}"));
        IsDead.Subscribe(isDead => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(isDead)} updated: {isDead}"));
    }

    public bool CanCastSpells()
    {
        return !IsDead.Value && _castingSpell.Value.Spell == null;
    }

    public SpellCastResult CastSpell(ActorSpellCastChoice spellCastChoice)
    {
        Debug.Assert(spellCastChoice.CastInfo.Caster == this);
        Debug.Assert(_spells.ContainsKey(spellCastChoice.SpellId));
        Debug.Assert(_castingSpell.Value.Spell == null);
        Debug.Assert(!IsDead.Value);

        if (!_spells.TryGetValue(spellCastChoice.SpellId, out var spell))
        {
            return SpellCastResult.Fail;
        }

        _timelineManager.AddSpellCastRequest(spell, spellCastChoice.CastInfo);
        _castingSpell.Value = (spell, spellCastChoice.CastInfo);
        return SpellCastResult.Success;
    }

    public void ProcessCastingEnded()
    {
        _castingSpell.Value = default;
    }

    public bool CanBeTargeted()
    {
        return !IsDead.Value;
    }

    public void ApplyDamage(DamageInfo damageInfo)
    {
        if (damageInfo.DamageAmount == 0)
        {
            return;
        }
        
        var piercedDamageAmount = damageInfo.PierceArmor
            ? damageInfo.DamageAmount
            : Math.Max(0, damageInfo.DamageAmount - _armor.Value);

        _hp.SetValueAndForceNotify(_hp.Value - piercedDamageAmount);
        var damageEventInfo = new DamageEventInfo(
            new DamageInfo(damageInfo.DamageSource, piercedDamageAmount, damageInfo.PierceArmor,
                damageInfo.ReturnedDamage),
            this
        );
        MessageBroker.Default.Publish(damageEventInfo);
    }

    public void ChangeArmor(float shiftAmount)
    {
        _armor.Value += shiftAmount;
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

    public override string ToString() => $"Actor '{Name}'";

    #region Debug

    public string GetRandomSpellId() => _spells.Keys.ElementAt(Random.Range(0, _spells.Count));

    #endregion

    public ActorSpellCastChoice GetAiSpellChoice(ActorAiBase.OuterWorldInfo outerWorldInfo)
    {
        return _ai.ChooseSpell(outerWorldInfo);
    }

    public void Dispose()
    {
        foreach (var spell in _spells.Values)
        {
            spell?.Dispose();
        }
    }
}
}