using Actors;

namespace Spells.Aspects
{
internal class HealAllySpellAspect : HealSpellAspect<HealAllySpellAspectConfig>
{
    public override SpellCastTarget CastTarget => SpellCastTarget.Ally;

    public HealAllySpellAspect(HealAllySpellAspectConfig config, string spellId) : base(config, spellId)
    { }

    protected override IActor GetHealTarget(SpellCastInfo castInfo) => castInfo.Target;
}
}