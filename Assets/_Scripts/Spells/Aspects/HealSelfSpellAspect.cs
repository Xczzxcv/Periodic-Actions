using Actors;

namespace Spells.Aspects
{
internal class HealSelfSpellAspect : HealSpellAspect<HealSelfSpellAspectConfig>
{
    public override SpellCastTarget CastTarget => SpellCastTarget.NoTarget;

    public HealSelfSpellAspect(HealSelfSpellAspectConfig config, string spellId) : base(config, spellId)
    { }

    protected override Actor GetHealTarget(SpellCastInfo castInfo) => castInfo.Caster;
}
}