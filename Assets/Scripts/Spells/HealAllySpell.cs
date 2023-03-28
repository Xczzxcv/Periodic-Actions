using SpellConfigs;

namespace Spells
{
internal class HealAllySpell : SpellBase<HealAllySpellConfig>
{
    public override bool IsTargeted => true;
    public override bool DamagePiercesArmor => false;
    public override bool CastedOnAllies => true;

    public HealAllySpell(HealAllySpellConfig config) : base(config)
    { }

    public override void MainCast(SpellCastInfo castInfo)
    {
        base.MainCast(castInfo);
        castInfo.Target.Heal(Config.HealAmount);
    }
}
}