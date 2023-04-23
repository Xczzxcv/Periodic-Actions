using System.Collections.Generic;
using Actors.Stats;

namespace Spells.Aspects
{
internal class DamageSpellAspect : SpellAspect<DamageSpellAspectConfig>
{
    public override SpellCastTarget CastTarget => SpellCastTarget.Enemy;

    public DamageSpellAspect(DamageSpellAspectConfig config, string spellId) : base(config, spellId)
    { }

    public override void InitialCast(SpellCastInfo castInfo)
    { }

    public override void MainCast(SpellCastInfo castInfo)
    {
        var damageAmount = Config.DamageAmount + castInfo.Caster.Stats.GetStat(ActorStat.DamageBuff);
        castInfo.Target.ApplyDamage(new DamageInfo(castInfo.Caster, damageAmount,
            Config.IsDamagePiercesArmor, false));
    }

    public override Dictionary<string, object> GetProperties()
    {
        return new Dictionary<string, object>
        {
            {Constants.SpellProperties.DAMAGE, Config.DamageAmount},
            {Constants.SpellProperties.IS_DAMAGE_PIERCES_ARMOR, Config.IsDamagePiercesArmor},
        };
    }

    public override void PostCast(SpellCastInfo castInfo)
    { }
}
}