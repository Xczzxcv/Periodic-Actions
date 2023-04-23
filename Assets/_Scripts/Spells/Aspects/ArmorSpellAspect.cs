using System.Collections.Generic;

namespace Spells.Aspects
{
internal class ArmorSpellAspect : SpellAspect<ArmorSpellAspectConfig>
{
    public override SpellCastTarget CastTarget => SpellCastTarget.NoTarget;

    public ArmorSpellAspect(ArmorSpellAspectConfig config, string spellId) : base(config, spellId)
    { }

    public override void InitialCast(SpellCastInfo castInfo)
    {
        castInfo.Caster.ChangeArmor(Config.ArmorAmount);
    }

    public override void MainCast(SpellCastInfo castInfo)
    { }

    public override Dictionary<string, object> GetProperties()
    {
        return new Dictionary<string, object>
        {
            {Constants.SpellProperties.ARMOR, Config.ArmorAmount},
        };
    }

    public override void PostCast(SpellCastInfo castInfo)
    {
        castInfo.Caster.ChangeArmor(-Config.ArmorAmount);
    }
}
}