namespace Actors.Ai
{
internal abstract class ActorAiBase<TConfig> : IActorAi
where TConfig : ActorAiBaseConfig
{
    protected readonly IActor Actor;
    protected readonly TConfig Config;

    protected ActorAiBase(IActor actor, TConfig config)
    {
        Actor = actor;
    }

    public abstract ActorSpellCastChoice ChooseSpell(IActorAi.OuterWorldInfo outerInfo);
}
}