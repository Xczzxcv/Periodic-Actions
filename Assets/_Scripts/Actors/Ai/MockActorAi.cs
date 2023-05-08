using System.Linq;
using UnityEngine;

namespace Actors.Ai
{
internal class MockActorAi : IActorAi
{
    private readonly IActor _actor;

    public MockActorAi(IActor actor)
    {
        _actor = actor;
    }
    
    public ActorSpellCastChoice ChooseSpell(IActorAi.OuterWorldInfo outerInfo)
    {
        Debug.LogError("You're using mock ai ");
        return ActorSpellCastChoice.Build(
            _actor.Spells.Spells.Keys.First(),
            _actor,
            outerInfo.EnemyTeam.Actors.First(),
            outerInfo.PreviousCastTime
        );
    }
}
}