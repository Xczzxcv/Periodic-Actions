using System.Collections.Generic;
using SpellConfigs;

namespace Spells
{
internal class HealSpell : SpellBase<HealSpellConfig>
{
    public override bool IsTargeted => true;
    protected override bool DamagePiercesArmor => false;

    public HealSpell(HealSpellConfig config) : base(config)
    { }

    public override void MainCast(SpellCastInfo castInfo)
    {
        base.MainCast(castInfo);
        castInfo.Caster.Heal(Config.HealAmount);
    }

    public override Dictionary<string, object> GetProperties()
    {
        var properties = base.GetProperties();
        properties.Add("heal", Config.HealAmount);

        return properties;
    }
}
}