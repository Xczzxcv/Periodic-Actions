using Spells.Aspects;

namespace Spells
{
internal class SpellAspectsFactory : ISpellAspectsFactory
{
    public ISpellAspect Create(SpellAspectConfig aspectConfig, string spellId)
    {
        return aspectConfig switch
        {
            DamageSpellAspectConfig dmgConfig => new DamageSpellAspect(dmgConfig, spellId),
            ArmorSpellAspectConfig tempArmorConfig => new ArmorSpellAspect(tempArmorConfig, spellId),
            HealSelfSpellAspectConfig healSelfConfig => new HealSelfSpellAspect(healSelfConfig, spellId),
            HealAllySpellAspectConfig healAllyConfig => new HealAllySpellAspect(healAllyConfig, spellId),
            ReturnDamageSpellAspectConfig returnDmgConfig => new ReturnDamageSpellAspect(returnDmgConfig, spellId),
            SelfStatShiftSpellAspectConfig selfStatShiftConfig => new SelfStatShiftSpellAspect(selfStatShiftConfig, spellId),
            _ => throw ThrowHelper.WrongConfigTypeInFactory<SpellAspectConfig, ISpellAspect>(aspectConfig),
        };
    }
}
}