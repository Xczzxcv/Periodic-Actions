using SpellConfigs;

namespace Spells
{
internal class DefenceSpell : SpellBase<DefenceSpellConfig>
{
    public override bool IsTargeted => false;
    protected override bool DamagePiercesArmor => false;

    public DefenceSpell(DefenceSpellConfig config) : base(config)
    { }
}
}