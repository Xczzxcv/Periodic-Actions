using Spells;

namespace Actors
{
internal readonly struct ActorSpellCastChoice
{
    public readonly string SpellId;
    public readonly SpellCastInfo CastInfo;

    public ActorSpellCastChoice(string spellId, SpellCastInfo castInfo)
    {
        SpellId = spellId;
        CastInfo = castInfo;
    }
}
}