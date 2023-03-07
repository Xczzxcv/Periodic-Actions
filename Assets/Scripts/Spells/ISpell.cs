using SpellConfigs;

namespace Spells
{
internal interface ISpell
{
    public string Id { get; }
    public SpellConfigBase Config { get; }
    public void PreCast(SpellCastInfo castInfo);
    public void Cast(SpellCastInfo castInfo);
}
}