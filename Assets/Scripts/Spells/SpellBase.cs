using System.Collections.Generic;
using Actors;
using SpellConfigs;
using UnityEngine;

namespace Spells
{
internal abstract class SpellBase<TSpellConfig> : ISpell
    where TSpellConfig : SpellConfigBase
{
    public string Id => Config.Id;
    public SpellConfigBase BaseConfig => Config;
    public abstract bool IsTargeted { get; }
    public abstract bool DamagePiercesArmor { get; }
    public abstract bool CastedOnAllies { get; }

    protected readonly TSpellConfig Config;

    protected SpellBase(TSpellConfig config)
    {
        Config = config;
    }

    public virtual void InitialCast(SpellCastInfo castInfo)
    {
        Debug.Assert(!IsTargeted || CastedOnAllies == Actor.IsAllies(castInfo.Caster, castInfo.Target),
            $"Wrong target side for '{Id}': {castInfo}");
        Debug.Assert(IsTargeted == (castInfo.Target != null), 
            $"Wrong target value for {Id}: {castInfo} != {IsTargeted}");
        
        castInfo.Caster.ChangeArmor(Config.Armor);
    }

    public virtual void MainCast(SpellCastInfo castInfo)
    {
        if (IsTargeted)
        {
            castInfo.Target.ApplyDamage(new DamageInfo(castInfo.Caster, Config.Damage,
                DamagePiercesArmor, false));
        }
    }

    public virtual void PostCast(SpellCastInfo castInfo)
    {
        castInfo.Caster.ChangeArmor(-Config.Armor);
    }

    public override string ToString() => $"Spell ({Config.Id})";

    public virtual Dictionary<string, object> GetProperties()
    {
        return new Dictionary<string, object>
        {
            {Constants.SpellProperties.DAMAGE, Config.Damage},
            {Constants.SpellProperties.ARMOR, Config.Armor},
            {Constants.SpellProperties.DELAY, Config.Delay},
        };
    }

    public virtual void Dispose()
    { }
}
}