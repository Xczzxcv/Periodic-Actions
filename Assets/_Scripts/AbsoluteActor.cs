using System;
using Actors.Ai;
using Inventory;
using UniRx;

namespace Actors
{
internal class AbsoluteActor : IActor
{
    public bool IsPlayerUnit => false;
    public bool IsAlive => true;
    public IReadOnlyReactiveProperty<ActorSide> Side => new ReactiveProperty<ActorSide>(ActorSide.Enemy);
    public IReadOnlyReactiveProperty<bool> IsDead => new ReactiveProperty<bool>(false);
    public string Name => "ABSOLUTE";
    public SpellsActorManager Spells => throw new NullReferenceException($"{GetType()} has no {nameof(Spells)}");
    public StatsActorManager Stats => throw new NullReferenceException($"{GetType()} has no {nameof(Stats)}");
    public ActorInventory Inventory { get; }
    public IActorAi Ai { get; }

    public AbsoluteActor()
    {
        Inventory = new ActorInventory(this);
        Ai = new MockActorAi(this);
    }

    public void Init(ActorConfig config)
    { }

    public bool CanBeTargeted() => false;

    public void ApplyDamage(DamageInfo damageInfo)
    { }

    public void Heal(double amount)
    { }

    public void ChangeArmor(double shiftAmount)
    { }

    public void Dispose()
    { }
}
}