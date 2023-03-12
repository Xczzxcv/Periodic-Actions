namespace Actors.Ai
{
internal interface IActorAi
{
    public abstract ActorSpellCastChoice ChooseSpell(ActorAiBase.OuterWorldInfo outerInfo);
}
}