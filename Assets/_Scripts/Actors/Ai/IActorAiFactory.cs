namespace Actors.Ai
{
internal interface IActorAiFactory
{
    IActorAi Create(ActorAiBaseConfig aiConfig, IActor actor);
}
}