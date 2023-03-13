using SpellConfigs;

namespace Spells
{
internal class PierceArmorHitSpell : SpellBase<PierceArmorHitSpellConfig>
{
    public override bool IsTargeted => true;
    protected override bool DamagePiercesArmor => true;

    public PierceArmorHitSpell(PierceArmorHitSpellConfig config) : base(config)
    { }
}
}