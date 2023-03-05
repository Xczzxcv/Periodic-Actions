using SpellConfigs;

namespace Spells
{
internal class DefenceSpell : SpellBase<DefenceSpellConfig>
{
    protected override bool DamagePiercesArmor => false;

    public DefenceSpell(DefenceSpellConfig spellConfig) : base(spellConfig)
    { }
}
}