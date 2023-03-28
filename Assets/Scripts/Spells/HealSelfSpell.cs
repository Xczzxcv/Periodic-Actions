using System.Collections.Generic;
using SpellConfigs;

namespace Spells
{
internal class HealSelfSpell : SpellBase<HealSelfSpellConfig>
{
    public override bool IsTargeted => false;
    public override bool DamagePiercesArmor => false;
    public override bool CastedOnAllies => false;

    public HealSelfSpell(HealSelfSpellConfig config) : base(config)
    { }

    public override void MainCast(SpellCastInfo castInfo)
    {
        base.MainCast(castInfo);
        castInfo.Caster.Heal(Config.HealAmount);
    }

    public override Dictionary<string, object> GetProperties()
    {
        var properties = base.GetProperties();
        properties.Add(Constants.SpellProperties.HEAL, Config.HealAmount);

        return properties;
    }
}
}