using System.Collections.Generic;
using System.Linq;
using Spells;
using UnityEngine;

namespace Actors.Ai
{
internal class RandomSetActorAi : ActorAiBase<RandomSetActorAiConfig>
{
    private readonly HashSet<ISpell> _spellsToUse = new();

    public RandomSetActorAi(IActor actor, RandomSetActorAiConfig config) : base(actor, config)
    {
        RefillSkillSet();
    }

    public override ActorSpellCastChoice ChooseSpell(IActorAi.OuterWorldInfo outerInfo)
    {
        var spellToUse = _spellsToUse.ElementAt(Random.Range(0, _spellsToUse.Count));
        _spellsToUse.Remove(spellToUse);
        if (_spellsToUse.Count == 0)
        {
            RefillSkillSet();
        }

        return RandomActorAi.RandomSpellCast(spellToUse, Actor, outerInfo);
    }

    private void RefillSkillSet()
    {
        foreach (var spell in Actor.Spells.Spells.Values)
        {
            _spellsToUse.Add(spell);
        }
    }
}
}