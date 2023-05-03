using System;
using Actors.Ai;
using Actors.Stats;
using Inventory;
using JetBrains.Annotations;
using Spells;
using UniRx;
using UnityEngine;

namespace Actors
{
internal class Actor : IDisposable
{
    public bool IsPlayerUnit => Side.Value == ActorSide.Player;
    public bool IsAlive => !IsDead.Value;

    public IReadOnlyReactiveProperty<ActorSide> Side => _side;
    public IReadOnlyReactiveProperty<bool> IsDead { get; private set; }
    [CanBeNull] public string Name { get; private set; }

    private readonly TimelineManager _timelineManager;

    private ReactiveProperty<ActorSide> _side;
    public readonly SpellsActorManager Spells;
    public readonly StatsActorManager Stats;
    public readonly ActorInventory Inventory;
    private readonly IActorAiFactory _actorAiFactory;
    private IActorAi _ai;

    public Actor(TimelineManager timelineManager, 
        ISpellsFactory spellsFactory,
        IStatsShiftFactory statsShiftFactory,
        IActorAiFactory actorAiFactory)
    {
        _timelineManager = timelineManager;
        Spells = new SpellsActorManager(timelineManager, spellsFactory, this);
        Stats = new StatsActorManager(statsShiftFactory, this);
        Inventory = new ActorInventory(this);
        _actorAiFactory = actorAiFactory;
    }

    public void Init(ActorConfig config)
    {
        Stats.Init(config);
        Spells.Init(config);
        _side = new (config.Side);
        IsDead = Stats.Hp.Select(hp => hp <= 0).ToReactiveProperty();
        _ai = config.Side == ActorSide.Enemy
            ? _actorAiFactory.Create(config.AiConfig, this)
            : new MockActorAi(this);

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

        Stats.Set(ActorStat.Hp, Stats.Hp.Value - piercedDamageAmount);
        var damageEventInfo = new DamageEventInfo(this, damageInfo, piercedDamageAmount);
        MessageBroker.Default.Publish(damageEventInfo);
    }

    public void Heal(double amount)
    {
        Debug.Assert(amount >= 0, "Negative heal amount");
        if (amount == 0)
        {
            return;
        }

        var healedHp = Math.Min(Stats.Hp.Value + amount, Stats.MaxHp.Value);
        Stats.Set(ActorStat.Hp, healedHp);
    }

    public void ChangeArmor(double shiftAmount)
    {
        Stats.Set(ActorStat.Armor, Stats.Armor.Value + shiftAmount);
    }

    public override string ToString() => $"Actor '{Name}'";

    public ActorSpellCastChoice GetAiSpellChoice(IActorAi.OuterWorldInfo outerWorldInfo)
    {
        return _ai.ChooseSpell(outerWorldInfo);
    }

    public static bool IsAllies([CanBeNull] Actor actor1, [CanBeNull] Actor actor2)
    {
        return actor1?.IsPlayerUnit == actor2?.IsPlayerUnit;
    }

    public void Dispose()
    {
        Spells.Dispose();
        Stats.Dispose();
    }
}
}