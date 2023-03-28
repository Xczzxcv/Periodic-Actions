using SpellConfigs;

namespace Spells
{
internal class HitSpell : SpellBase<HitSpellConfig>
{
    public override bool IsTargeted => true;
    public override bool DamagePiercesArmor => false;
    public override bool CastedOnAllies => false;

    public HitSpell(HitSpellConfig config) : base(config)
    { }
}
}