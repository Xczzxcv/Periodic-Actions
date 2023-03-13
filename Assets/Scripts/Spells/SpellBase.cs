using System.Collections.Generic;
using SpellConfigs;

namespace Spells
{
internal abstract class SpellBase<TSpellConfig> : ISpell
    where TSpellConfig : SpellConfigBase
{
    public string Id => Config.Id;
    public SpellConfigBase BaseConfig => Config;
    public abstract bool IsTargeted { get; }
    protected abstract bool DamagePiercesArmor { get; }

    protected readonly TSpellConfig Config;

    protected SpellBase(TSpellConfig config)
    {
        Config = config;
    }

    public virtual void InitialCast(SpellCastInfo castInfo)
    {
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
            {"damage", Config.Damage},
            {"armor", Config.Armor},
            {"duration", Config.Duration},
        };
    }

    public virtual void Dispose()
    { }
}
}