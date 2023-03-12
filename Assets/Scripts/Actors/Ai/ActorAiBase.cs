namespace Actors.Ai
{
internal abstract class ActorAiBase : IActorAi
{
    internal struct OuterWorldInfo
    {
        public ActorsTeam AllyTeam;
        public ActorsTeam EnemyTeam;
        public double PreviousCastTime;
    }

    protected readonly Actor Actor;

    protected ActorAiBase(Actor actor)
    {
        Actor = actor;
    }

    public abstract ActorSpellCastChoice ChooseSpell(OuterWorldInfo outerInfo);
}
}