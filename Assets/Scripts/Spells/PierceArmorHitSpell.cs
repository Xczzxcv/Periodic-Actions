using SpellConfigs;

namespace Spells
{
internal class PierceArmorHitSpell : SpellBase<PierceArmorHitSpellConfig>
{
    protected override bool DamagePiercesArmor => true;

    public PierceArmorHitSpell(PierceArmorHitSpellConfig spellConfig) : base(spellConfig)
    { }
}
}