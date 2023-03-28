using System;
using System.Linq;
using JetBrains.Annotations;
using Spells;
using Random = UnityEngine.Random;

namespace Actors.Ai
{
internal class RandomActorAi : ActorAiBase
{
    public RandomActorAi(Actor actor) : base(actor)
    { }

    public override ActorSpellCastChoice ChooseSpell(OuterWorldInfo outerInfo)
    {
        var randomSpell = Actor.Spells.GetRandomSpell();
        if (!randomSpell.IsTargeted)
        {
            return CastSpellInternal(outerInfo, randomSpell.Id, null);
        }

        var targetTeam = randomSpell.CastedOnAllies
            ? outerInfo.AllyTeam
            : outerInfo.EnemyTeam;
        if (targetTeam.Actors.All(actor => !actor.CanBeTargeted()))
        {
            var notTargetedSpells = Actor.Spells.Spells.Values.Where(spell => !spell.IsTargeted);
            if (notTargetedSpells.Any())
            {
                return new ActorSpellCastChoice(
                    notTargetedSpells.First().Id,
                    new SpellCastInfo(outerInfo.PreviousCastTime, Actor, null)
                );
            }

            throw new ArgumentException($"Can't find spell and target for {Actor} ");
        }

        Actor randomTargetActor;
        do
        {
            randomTargetActor = targetTeam.Actors[Random.Range(0, targetTeam.Actors.Count)];
        } while (!randomTargetActor.CanBeTargeted());

        var spellTarget = randomSpell.IsTargeted
            ? randomTargetActor
            : null;

        return new ActorSpellCastChoice(
            randomSpell.Id,
            new SpellCastInfo(outerInfo.PreviousCastTime, Actor, spellTarget)
        );
    }

    private ActorSpellCastChoice CastSpellInternal(OuterWorldInfo outerInfo, string spellId,
        [CanBeNull] Actor target)
    {
        return new ActorSpellCastChoice(
            spellId,
            new SpellCastInfo(outerInfo.PreviousCastTime, Actor, target)
        );
    }
}
}