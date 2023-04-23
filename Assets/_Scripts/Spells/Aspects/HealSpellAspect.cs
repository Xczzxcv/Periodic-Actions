using System.Collections.Generic;
using Actors;
using Actors.Stats;

namespace Spells.Aspects
{
internal abstract class HealSpellAspect<TConfig> : SpellAspect<TConfig> 
    where  TConfig : HealSpellAspectConfig
{
    protected HealSpellAspect(TConfig config, string spellId) : base(config, spellId)
    { }

    public override void InitialCast(SpellCastInfo castInfo)
    { }

    public override void MainCast(SpellCastInfo castInfo)
    {
        var healTarget = GetHealTarget(castInfo);
        var healAmount = Config.HealAmount + castInfo.Caster.Stats.GetStat(ActorStat.HealBuff);
        healTarget.Heal(healAmount);
    }

    public override void PostCast(SpellCastInfo castInfo)
    { }

    protected abstract Actor GetHealTarget(SpellCastInfo castInfo);

    public override Dictionary<string, object> GetProperties()
    {
        return new Dictionary<string, object>
        {
            {Constants.SpellProperties.HEAL, Config.HealAmount}
        };
    }
}
}