namespace Actors.Ai
{
internal interface IActorAi
{
    internal struct OuterWorldInfo
    {
        public ActorsTeam AllyTeam;
        public ActorsTeam EnemyTeam;
        public int PreviousCastTime;
    }
    
    public ActorSpellCastChoice ChooseSpell(OuterWorldInfo outerInfo);
}
}