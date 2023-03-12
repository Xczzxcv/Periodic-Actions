namespace Actors.Ai
{
internal abstract class ActorAiBase : IActorAi
{
    internal struct OuterWorldInfo
    { }

    protected readonly Actor Actor;

    protected ActorAiBase(Actor actor)
    {
        Actor = actor;
    }

    public abstract void CastSpell(OuterWorldInfo info);
}
}