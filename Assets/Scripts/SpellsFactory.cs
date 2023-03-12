using System;
using System.Collections.Generic;
using SpellConfigs;
using Spells;
using UnityEngine;

internal class SpellsFactory : MonoBehaviour
{
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
    private List<SpellConfigBase> spellConfigs;
    
    public readonly Dictionary<string, SpellConfigBase> _spellConfigsCollection = new();

    private void Awake()
    {
        foreach (var spellConfig in spellConfigs)
        {
            _spellConfigsCollection.Add(spellConfig.Id, spellConfig);
        }
    }

    public ISpell BuildSpell(string spellId)
    {
        var spellConfig = _spellConfigsCollection[spellId];
        return spellConfig switch
        {
            HitSpellConfig hitSpellConfig => new HitSpell(hitSpellConfig),
            DefenceSpellConfig defenceSpellConfig => new DefenceSpell(defenceSpellConfig),
            PierceArmorHitSpellConfig pierceArmorHitSpellConfig => new PierceArmorHitSpell(pierceArmorHitSpellConfig),
            ReturnDamageSpellConfig returnDamageSpellConfig => new ReturnDamageSpell(returnDamageSpellConfig),
            _ => throw new ArgumentException($"Unknown spell config type '{spellConfig.Id}'")
        };
    }
}