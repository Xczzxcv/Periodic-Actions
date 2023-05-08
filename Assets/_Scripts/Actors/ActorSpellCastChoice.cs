using JetBrains.Annotations;
using Spells;

namespace Actors
{
internal readonly struct ActorSpellCastChoice
{
    public readonly string SpellId;
    public readonly SpellCastInfo CastInfo;

    private ActorSpellCastChoice(string spellId, SpellCastInfo castInfo)
    {
        SpellId = spellId;
        CastInfo = castInfo;
    }

    public static ActorSpellCastChoice Build(string spellId, IActor caster, [CanBeNull] IActor target,
        int previousCastTime)
    {
        return new ActorSpellCastChoice(
            spellId,
            new SpellCastInfo(previousCastTime, caster, target)
        );
    }
}
}