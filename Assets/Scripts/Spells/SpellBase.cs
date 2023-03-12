using SpellConfigs;

namespace Spells
{
internal abstract class SpellBase<TSpellConfig> : ISpell
    where TSpellConfig : SpellConfigBase
{
    public string Id => SpellConfig.Id;
    public SpellConfigBase Config => SpellConfig;
    protected abstract bool DamagePiercesArmor { get; }

    protected readonly TSpellConfig SpellConfig;

    protected SpellBase(TSpellConfig spellConfig)
    {
        SpellConfig = spellConfig;
    }

    public virtual void InitialCast(SpellCastInfo castInfo)
    {
        castInfo.Caster.ChangeArmor(SpellConfig.Armor);
    }

    public virtual void MainCast(SpellCastInfo castInfo)
    {
        castInfo.Target.ApplyDamage(new DamageInfo(castInfo.Caster, SpellConfig.Damage, 
            DamagePiercesArmor, false));
    }

    public virtual void PostCast(SpellCastInfo castInfo)
    {
        castInfo.Caster.ChangeArmor(-SpellConfig.Armor);
    }

    public override string ToString() => $"Spell ({SpellConfig.Id})";

    public virtual void Dispose()
    { }
}
}