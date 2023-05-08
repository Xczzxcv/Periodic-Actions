using Actors;

namespace Spells.Aspects
{
internal class SelfStatShiftSpellAspect : StatShiftSpellAspect<SelfStatShiftSpellAspectConfig>
{
    public override SpellCastTarget CastTarget => SpellCastTarget.NoTarget;

    public SelfStatShiftSpellAspect(SelfStatShiftSpellAspectConfig config, string spellId) : base(config, spellId)
    { }

    protected override IActor GetStatsShiftTarget(SpellCastInfo spellCastInfo) => spellCastInfo.Caster;
}
}