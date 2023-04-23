using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Spells.Aspects
{
internal class ReturnDamageSpellAspect : SpellAspect<ReturnDamageSpellAspectConfig>
{
    public override SpellCastTarget CastTarget => SpellCastTarget.NoTarget;
    
    private IDisposable _subscription;

    public ReturnDamageSpellAspect(ReturnDamageSpellAspectConfig config, string spellId) : base(config, spellId)
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
                false,
                true
            ));
        });
    }

    public override void MainCast(SpellCastInfo castInfo)
    { }

    public override void PostCast(SpellCastInfo castInfo)
    {
        _subscription.Dispose();
        _subscription = null;
    }

    public override Dictionary<string, object> GetProperties()
    {
        return new Dictionary<string, object>
        {
            { Constants.SpellProperties.RETURN_DAMAGE, true },
        };
    }

    public override void Dispose()
    {
        _subscription?.Dispose();
        base.Dispose();
    }
}
}