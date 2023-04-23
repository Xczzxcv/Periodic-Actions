using System.Collections.Generic;
using System.Linq;
using Actors;

internal class ActorsTeam
{
    public IReadOnlyList<Actor> Actors => _actors;
    public bool IsAlive => _actors.Any(actor => !actor.IsDead.Value);

    private readonly List<Actor> _actors;

    public ActorsTeam(int initTeamCapacity = 0)
    {
        _actors = new(initTeamCapacity);
    }

    public void AddMember(Actor member) => _actors.Add(member);

    public bool ContainsMember(Actor actor) => _actors.Contains(actor);
}