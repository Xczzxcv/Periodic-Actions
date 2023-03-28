using System;
using Actors.Ai;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

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

    public IReadOnlyReactiveProperty<ActorSide> Side => _side;
    public IReadOnlyReactiveProperty<bool> IsDead { get; private set; }
    [CanBeNull] public string Name { get; private set; }

    private readonly TimelineManager _timelineManager;

    private ReactiveProperty<ActorSide> _side;
    public readonly SpellsActorManager Spells;
    public readonly StatsActorManager Stats;
    private IActorAi _ai;

    public Actor(TimelineManager timelineManager, SpellsFactory spellsFactory)
    {
        _timelineManager = timelineManager;
        Spells = new SpellsActorManager(timelineManager, spellsFactory, this);
        Stats = new StatsActorManager(this);
    }

    public void Init(Config config)
    {
        Stats.Init(config);
        Spells.Init(config);
        _side = new (config.Side);
        IsDead = Stats.Hp.Select(hp => hp <= 0).ToReactiveProperty();
        _ai = new RandomActorAi(this);
        Name = config.Name;

        Stats.Hp.Subscribe(hp => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(hp)} updated: {hp}"));
        Stats.Armor.Subscribe(armor => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(armor)} updated: {armor}"));
        IsDead.Subscribe(isDead => Debug.Log($"[{_timelineManager.CurrentTime}] {this}) {nameof(isDead)} updated: {isDead}"));
    }

    public bool CanBeTargeted()
    {
        return !IsDead.Value;
    }

    public void ApplyDamage(DamageInfo damageInfo)
    {
        Debug.Assert(damageInfo.DamageAmount >= 0, $"Negative damage in {damageInfo}");
        if (damageInfo.DamageAmount == 0)
        {
            return;
        }

        var piercedDamageAmount = damageInfo.PierceArmor
            ? damageInfo.DamageAmount
            : Math.Max(0, damageInfo.DamageAmount - Stats.Armor.Value);

        Stats.Set(Constants.Stats.HP, Stats.Hp.Value - piercedDamageAmount);
        var damageEventInfo = new DamageEventInfo(this, damageInfo, piercedDamageAmount);
        MessageBroker.Default.Publish(damageEventInfo);
    }

    public void Heal(float amount)
    {
        Debug.Assert(amount >= 0, "Negative heal amount");
        if (amount == 0)
        {
            return;
        }

        var healedHp = Math.Min(Stats.Hp.Value + amount, Stats.MaxHp.Value);
        Stats.Set(Constants.Stats.HP, healedHp);
    }

    public void ChangeArmor(double shiftAmount)
    {
        Stats.Set(Constants.Stats.ARMOR, Stats.Armor.Value + shiftAmount);
    }

    public override string ToString() => $"Actor '{Name}'";

    public ActorSpellCastChoice GetAiSpellChoice(ActorAiBase.OuterWorldInfo outerWorldInfo)
    {
        return _ai.ChooseSpell(outerWorldInfo);
    }

    public static bool IsAllies(Actor actor1, Actor actor2)
    {
        return actor1.IsPlayerUnit == actor2.IsPlayerUnit;
    }

    public void Dispose()
    {
        Spells.Dispose();
    }
}
}