using System.Collections.Generic;
using SpellConfigs;

namespace Spells
{
internal interface ISpell
{
    public string Id { get; }
    public SpellConfigBase BaseConfig { get; }
    public bool IsTargeted { get; }
    public bool DamagePiercesArmor { get; }
    public bool CastedOnAllies { get; }
    public void InitialCast(SpellCastInfo castInfo);
    public void MainCast(SpellCastInfo castInfo);
    public void PostCast(SpellCastInfo castInfo);
    public void Dispose();
    Dictionary<string, object> GetProperties();
}
}