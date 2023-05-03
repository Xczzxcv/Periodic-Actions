namespace Actors.Ai
{
internal class ActorAiFactory : IActorAiFactory
{
    public IActorAi Create(ActorAiBaseConfig aiConfig, Actor actor)
    {
        return aiConfig switch
        {
            RandomAiConfig randomConfig => new RandomActorAi(actor, randomConfig),
            RandomSetActorAiConfig randomSetConfig => new RandomSetActorAi(actor, randomSetConfig),
            _ => throw ThrowHelper.WrongConfigTypeInFactory<ActorAiBaseConfig, IActorAi>(aiConfig),
        };
    }
}
}