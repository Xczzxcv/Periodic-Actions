using SpellConfigs;

namespace Spells
{
internal class HitSpell : SpellBase<HitSpellConfig>
{
    public override bool IsTargeted => true;
    protected override bool DamagePiercesArmor => false;

    public HitSpell(HitSpellConfig config) : base(config)
    { }
}
}