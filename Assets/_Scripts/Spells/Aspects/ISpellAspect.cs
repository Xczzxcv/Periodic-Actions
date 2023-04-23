using System;

namespace Spells.Aspects
{
internal interface ISpellAspect : IDisposable, IHaveProperties
{
    string SpellId { get; }
    SpellCastTarget CastTarget { get; }
    void InitialCast(SpellCastInfo castInfo);
    void MainCast(SpellCastInfo castInfo);
    void PostCast(SpellCastInfo castInfo);
}
}