using System;
using SpellConfigs;
using UniRx;
using UnityEngine;

namespace Spells
{
internal class ReturnDamageSpell : SpellBase<ReturnDamageSpellConfig>
{
    public override bool IsTargeted => false;
    public override bool DamagePiercesArmor => false;
    public override bool CastedOnAllies => false;

    private IDisposable _subscription;
    
    public ReturnDamageSpell(ReturnDamageSpellConfig config) : base(config)
    { }

    public override void InitialCast(SpellCastInfo castInfo)
    {
        Debug.Assert(_subscription == null, $"Subscription is not null {_subscription}");

        _subscription = MessageBroker.Default.Receive<DamageEventInfo>().Subscribe(damageEvent =>
        {
            if (damageEvent.DamageTarget != castInfo.Caster)
            {
                return;
            }

            if (damageEvent.DamageInfo.ReturnedDamage)
            {
                return;
            }
            
            damageEvent.DamageInfo.DamageSource.ApplyDamage(new DamageInfo(
                castInfo.Caster,
                damageEvent.DamageInfo.DamageAmount,
                DamagePiercesArmor,
                true
            ));
        });

        base.InitialCast(castInfo);
    }

    public override void PostCast(SpellCastInfo castInfo)
    {
        base.PostCast(castInfo);
        _subscription.Dispose();
        _subscription = null;
    }

    public override void Dispose()
    {
        _subscription?.Dispose();
        base.Dispose();
    }
}
}