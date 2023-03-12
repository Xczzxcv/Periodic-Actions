using SpellConfigs;

namespace Spells
{
internal interface ISpell
{
    public string Id { get; }
    public SpellConfigBase Config { get; }
    public bool IsTargeted { get; }
    public void InitialCast(SpellCastInfo castInfo);
    public void PostCast(SpellCastInfo castInfo);
    public void MainCast(SpellCastInfo castInfo);
    public void Dispose();
}
}