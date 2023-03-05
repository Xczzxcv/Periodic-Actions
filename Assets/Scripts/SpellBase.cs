using SpellConfigs;

namespace Spells
{
internal abstract class SpellBase<TSpellConfig> : ISpell
    where TSpellConfig : SpellConfigBase
{
    public string Id => SpellConfig.Id;
    public float Duration => SpellConfig.Duration;
    protected abstract bool DamagePiercesArmor { get; }

    protected readonly TSpellConfig SpellConfig;

    protected SpellBase(TSpellConfig spellConfig)
    {
        SpellConfig = spellConfig;
    }

    public void PreCast(SpellCastInfo castInfo)
    {
        castInfo.Caster.ChangeArmor(SpellConfig.Armor);
    }

    public virtual void Cast(SpellCastInfo castInfo)
    {
        castInfo.Target.ApplyDamage(SpellConfig.Damage, DamagePiercesArmor);
        castInfo.Caster.ChangeArmor(-SpellConfig.Armor);
    }

    public override string ToString() => $"Spell ({SpellConfig.Id})";
}
}