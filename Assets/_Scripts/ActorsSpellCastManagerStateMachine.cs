internal class ActorsSpellCastManagerStateMachine : StateMachine<ActorsSpellCastManager>
{
    public ActorsSpellCastManagerStateMachine(
        TimelineManager timelineManager, 
        ActorsManager actorsManager)
    {
        States.Add(typeof(IdleState), new IdleState(this, timelineManager, actorsManager));
        States.Add(typeof(CastingSpellState), new CastingSpellState(this, timelineManager, actorsManager));
    }
}