internal class ActorsSpellCastManager
{
    private readonly ActorsSpellCastManagerStateMachine _stateMachine;

    private bool _currentlyCastingSpell;
    
    public int UnitsQueueSize => _stateMachine.GetState<IdleState>().UnitsQueueSize;

    public ActorsSpellCastManager(TimelineManager timelineManager, ActorsManager actorsManager)
    {
        _stateMachine = new ActorsSpellCastManagerStateMachine(timelineManager, actorsManager);
        _stateMachine.EnterState<IdleState>();
    }

    public void TeamSpellCast(ActorsTeam castersTeam, double castTime)
    {
        foreach (var caster in castersTeam.Actors)
        {
            _stateMachine.GetState<IdleState>().AddToCastQueue(caster, castTime);
        }
    }

    public void Update()
    {
        _stateMachine.Update();
    }

    public void Dispose()
    {
        _stateMachine.Dispose();
    }
}