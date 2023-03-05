using SpellConfigs;

namespace Spells
{
internal class HitSpell : SpellBase<HitSpellConfig>
{
    protected override bool DamagePiercesArmor => false;

    public HitSpell(HitSpellConfig spellConfig) : base(spellConfig)
    { }
}
}