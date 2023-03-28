using SpellConfigs;

namespace Spells
{
internal class PierceArmorHitSpell : SpellBase<PierceArmorHitSpellConfig>
{
    public override bool IsTargeted => true;
    public override bool DamagePiercesArmor => true;
    public override bool CastedOnAllies => false;

    public PierceArmorHitSpell(PierceArmorHitSpellConfig config) : base(config)
    { }
}
}