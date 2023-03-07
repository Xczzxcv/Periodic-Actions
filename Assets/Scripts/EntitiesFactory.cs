using System;
using SpellConfigs;
using Spells;

internal static class EntitiesFactory
{
    public static ISpell BuildSpell<TSpellConfig>(TSpellConfig spellConfig) 
        where TSpellConfig : SpellConfigBase
    {
        return spellConfig switch
        {
            HitSpellConfig hitSpellConfig => new HitSpell(hitSpellConfig),
            DefenceSpellConfig defenceSpellConfig => new DefenceSpell(defenceSpellConfig),
            PierceArmorHitSpellConfig pierceArmorHitSpellConfig => new PierceArmorHitSpell(pierceArmorHitSpellConfig),
            _ => throw new ArgumentException($"Unknown spell config type '{spellConfig.Id}'")
        };
    }
}