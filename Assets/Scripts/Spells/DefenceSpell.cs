using SpellConfigs;

namespace Spells
{
internal class DefenceSpell : SpellBase<DefenceSpellConfig>
{
    public override bool IsTargeted => false;
    public override bool DamagePiercesArmor => false;
    public override bool CastedOnAllies => false;

    public DefenceSpell(DefenceSpellConfig config) : base(config)
    { }
}
}