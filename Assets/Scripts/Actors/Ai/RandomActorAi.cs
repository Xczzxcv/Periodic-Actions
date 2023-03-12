using System;
using System.Linq;
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
        var targetTeam = outerInfo.EnemyTeam;

        if (targetTeam.Actors.All(actor => !actor.CanBeTargeted()))
        {
            var notTargetedSpells = Actor.Spells.Values.Where(spell => !spell.IsTargeted);
            if (notTargetedSpells.Any())
            {
                return new ActorSpellCastChoice(
                    notTargetedSpells.First().Id,
                    new SpellCastInfo(outerInfo.PreviousCastTime, Actor, null)
                );
            }

            throw new ArgumentException($"Can't find spell and target for {Actor} ");
        }

        Actor randomEnemyActor;
        do
        {
            randomEnemyActor = targetTeam.Actors[Random.Range(0, targetTeam.Actors.Count)];
        } while (!randomEnemyActor.CanBeTargeted());

        return new ActorSpellCastChoice(
            Actor.GetRandomSpellId(),
            new SpellCastInfo(outerInfo.PreviousCastTime, Actor, randomEnemyActor)
        );
    }
}
}