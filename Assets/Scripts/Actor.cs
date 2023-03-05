using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

internal class Actor : ISpellCaster, ISpellTarget
{
    [Serializable]
    internal struct Config
    {
        public string Name;
        public float Hp;
        public float Armor;
    }

    public IReadOnlyReactiveProperty<float> Hp => _hp;
    public IReadOnlyReactiveProperty<float> Armor => _armor;
    public IReadOnlyReactiveProperty<bool> IsBusy => _isBusy;
    public IReadOnlyReactiveProperty<bool> IsDead { get; }

    private readonly ReactiveProperty<float> _hp = new();
    private readonly ReactiveProperty<float> _armor = new();
    private readonly ReactiveProperty<bool> _isBusy = new(false);
    private readonly Dictionary<string, ISpell> _spells = new();
    private readonly TimelineManager _timelineManager;
    private string _name;
    [CanBeNull]
    private IDisposable _skillUsageSubscription;

    public static Actor Build(TimelineManager timelineManager, Config config, params ISpell[] spells)
    {
        var newActor = new Actor(timelineManager);
        newActor.Init(config, spells);

        return newActor;
    }

    private Actor(TimelineManager timelineManager)
    {
        _timelineManager = timelineManager;

        IsDead = _hp.Select(hp => hp <= 0).ToReactiveProperty();
    }

    private void Init(Config config, params ISpell[] spells)
    {
        _hp.Value = config.Hp;
        _armor.Value = config.Armor;
        _name = config.Name;
        foreach (var spell in spells)
        {
            AddSpell(spell);
        }

        _hp.Subscribe(hp => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(hp)} updated: {hp}"));
        _armor.Subscribe(armor => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(armor)} updated: {armor}"));
        IsDead.Subscribe(isDead => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(isDead)} updated: {isDead}"));
    }

    public SpellCastResult CastSpell(string spellId, SpellCastInfo castInfo)
    {
        Debug.Assert(castInfo.Caster == this);
        Debug.Assert(_spells.ContainsKey(spellId));
        Debug.Assert(!_isBusy.Value);

        if (!_spells.TryGetValue(spellId, out var spell))
        {
            return SpellCastResult.Fail;
        }

        _timelineManager.AddSpellCastRequest(spell, castInfo);
        _isBusy.Value = true;

        _skillUsageSubscription?.Dispose();
        _skillUsageSubscription = _timelineManager.CastedSpellInfo.Subscribe(deferredCastInfo =>
        {
            if (deferredCastInfo.CastInfo.Equals(castInfo)
                && deferredCastInfo.Spell == spell)
            {
                _isBusy.Value = false;
            }
        });
        return SpellCastResult.Success;
    }

    public void ApplyDamage(float damageAmount, bool pierceArmor = false)
    {
        float piercedDamageAmount;
        if (pierceArmor)
        {
            piercedDamageAmount = damageAmount;
        }
        else
        {
            piercedDamageAmount = damageAmount - _armor.Value;
        }

        _hp.SetValueAndForceNotify(_hp.Value - piercedDamageAmount);
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

    public override string ToString() => $"Actor '{_name}'";


    #region Debug

    public string GetRandomSpellId() => _spells.Keys.ElementAt(Random.Range(0, _spells.Count));

    #endregion
}
