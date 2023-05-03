namespace Actors.Ai
{
internal interface IActorAi
{
    internal struct OuterWorldInfo
    {
        public ActorsTeam AllyTeam;
        public ActorsTeam EnemyTeam;
        public double PreviousCastTime;
    }
    
    public abstract ActorSpellCastChoice ChooseSpell(OuterWorldInfo outerInfo);
}
}