using System.Collections.Generic;
using Actors;

namespace Spells.Aspects
{
internal abstract class StatShiftSpellAspect<TConfig> : SpellAspect<TConfig>
    where TConfig : StatShiftSpellAspectConfig
{
    protected StatShiftSpellAspect(TConfig config, string spellId) : base(config, spellId)
    { }

    public override void InitialCast(SpellCastInfo castInfo)
    { }

    public override void MainCast(SpellCastInfo castInfo)
    {
        var target = GetStatsShiftTarget(castInfo);
        target.Stats.AddStatShift(Config.Stats, SpellId);
    }

    public override void PostCast(SpellCastInfo castInfo)
    { }

    protected abstract Actor GetStatsShiftTarget(SpellCastInfo spellCastInfo);

    public override Dictionary<string, object> GetProperties()
    {
        return Config.Stats.GetProperties();
    }
}
}