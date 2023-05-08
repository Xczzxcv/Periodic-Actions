using System;
using Actors.Ai;
using Inventory;
using UniRx;

namespace Actors
{
internal interface IActor : IDisposable
{
    bool IsPlayerUnit { get; }
    bool IsAlive { get; }
    IReadOnlyReactiveProperty<ActorSide> Side { get; }
    IReadOnlyReactiveProperty<bool> IsDead { get; }
    string Name { get; }
    SpellsActorManager Spells{get;}
    StatsActorManager Stats{get;}
    ActorInventory Inventory{get;}
    IActorAi Ai { get; }
    void Init(ActorConfig config);
    bool CanBeTargeted();
    void ApplyDamage(DamageInfo damageInfo);
    void Heal(double amount);
    void ChangeArmor(double shiftAmount);
}
}