using System;
using System.Collections.Generic;

namespace Spells
{
internal interface ISpell : IDisposable
{
    public string Id { get; }
    public SpellCastTarget CastTarget { get; }

    public void InitialCast(SpellCastInfo castInfo);
    public void MainCast(SpellCastInfo castInfo);
    public void PostCast(SpellCastInfo castInfo);
    public double GetDelay();
    Dictionary<string, object> GetProperties();
}
}