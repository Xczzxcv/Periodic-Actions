using System;
using Actors;
using UniRx;

internal class IdleState : State<ActorsSpellCastManager>, IUpdatableState<ActorsSpellCastManager>
{
    private readonly TimelineManager _timelineManager;
    private readonly ActorsManager _actorsManager;
    private readonly SpellCastQueue _unitsWaitingList = new();
    private IDisposable _spellPostCastSubscription;

    public int UnitsQueueSize => _unitsWaitingList.Count;

    public IdleState(StateMachine<ActorsSpellCastManager> stateMachine, 
        TimelineManager timelineManager,
        ActorsManager actorsManager) 
        : base(stateMachine)
    {
        _timelineManager = timelineManager;
        _actorsManager = actorsManager;
    }

    public override void Enter()
    {
        _spellPostCastSubscription = _timelineManager.PostCastedSpellInfo
            .SkipLatestValueOnSubscribe()
            .Subscribe(OnNextSpellPostCast);
    }

    private void OnNextSpellPostCast(DeferredSpellCastInfo deferredCastInfo)
    {
        var caster = deferredCastInfo.CastInfo.Caster;
        var previousCastTime = deferredCastInfo.CastTime;

        if (caster == null)
        {
            return;
        }

        if (_actorsManager.IsGameEnded())
        {
            return;
        }

        AddToCastQueue(caster, previousCastTime);
    }

    public void AddToCastQueue(Actor caster, double previousCastTime)
    {
        _unitsWaitingList.Enqueue((caster, previousCastTime));
    }

    public void Update()
    {
        if (_unitsWaitingList.Count <= 0)
        {
            return;
        }

        StateMachine.EnterStateWithArgs<CastingSpellState, CastingSpellState.Args>(new CastingSpellState.Args
        {
            Queue = _unitsWaitingList,
        });
    }

    public override void Exit()
    {
        _spellPostCastSubscription?.Dispose();
    }

    public override void Dispose()
    {
        _spellPostCastSubscription?.Dispose();
    }
}