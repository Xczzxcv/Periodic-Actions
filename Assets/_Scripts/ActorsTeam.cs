using System.Collections.Generic;
using System.Linq;
using Actors;

internal class ActorsTeam
{
    public IReadOnlyList<IActor> Actors => _actors;
    public bool IsAlive => _actors.Any(actor => !actor.IsDead.Value);

    private readonly List<IActor> _actors;

    public ActorsTeam(int initTeamCapacity = 0)
    {
        _actors = new(initTeamCapacity);
    }

    public void AddMember(IActor member) => _actors.Add(member);

    public bool ContainsMember(IActor actor) => _actors.Contains(actor);
}