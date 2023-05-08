using System;
using System.Linq;
using Spells;
using Random = UnityEngine.Random;

namespace Actors.Ai
{
internal class RandomActorAi : ActorAiBase<RandomAiConfig>
{
    public RandomActorAi(IActor actor, RandomAiConfig config) : base(actor, config)
    { }

    public override ActorSpellCastChoice ChooseSpell(IActorAi.OuterWorldInfo outerInfo)
    {
        var randomSpell = Actor.Spells.GetRandomSpell();
        return RandomSpellCast(randomSpell, Actor, outerInfo);
    }

    public static ActorSpellCastChoice RandomSpellCast(ISpell spell, IActor caster, 
        IActorAi.OuterWorldInfo outerInfo)
    {
        if (spell.CastTarget == SpellCastTarget.NoTarget)
        {
            return ActorSpellCastChoice.Build( spell.Id, caster, null, outerInfo.PreviousCastTime);
        }

        var targetTeam = spell.CastTarget == SpellCastTarget.Ally
            ? outerInfo.AllyTeam
            : outerInfo.EnemyTeam;
        if (targetTeam.Actors.All(actor => !actor.CanBeTargeted()))
        {
            var notTargetedSpells = caster.Spells.Spells.Values
                .Where(spell1 => spell1.CastTarget == SpellCastTarget.NoTarget)
                .ToArray();
            if (notTargetedSpells.Any())
            {
                return ActorSpellCastChoice.Build(
                    notTargetedSpells.First().Id,
                    caster,
                    null,
                    outerInfo.PreviousCastTime
                );
            }

            throw new ArgumentException($"Can't find spell and target for {caster} ");
        }

        const int saveCntMax = 300;
        var saveCnt = 0;
        IActor randomTargetActor;
        do
        {
            randomTargetActor = targetTeam.Actors[Random.Range(0, targetTeam.Actors.Count)];
        } while (!randomTargetActor.CanBeTargeted()
                 && saveCnt++ < saveCntMax);

        var spellTarget = spell.CastTarget != SpellCastTarget.NoTarget
            ? randomTargetActor
            : null;

        return ActorSpellCastChoice.Build(spell.Id, caster, spellTarget, outerInfo.PreviousCastTime);
    }
}
}