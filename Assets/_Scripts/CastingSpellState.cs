using System;
using System.Collections.Generic;
using Actors;
using Spells;
using UniRx;

internal class CastingSpellState : StateWithArgs<ActorsSpellCastManager, CastingSpellState.Args>
{
    private readonly Dictionary<Actor, IDisposable> _actorSpellUsageSubscriptions = new();
    private readonly TimelineManager _timelineManager;
    private readonly ActorsManager _actorsManager;
    private SpellCastQueue _spellCastQueue;

    internal struct Args
    {
        public SpellCastQueue Queue;
    }

    public CastingSpellState(StateMachine<ActorsSpellCastManager> stateMachine,
        TimelineManager timelineManager,
        ActorsManager actorsManager) 
        : base(stateMachine)
    {
        _actorsManager = actorsManager;
        _timelineManager = timelineManager;
    }

    public override void Enter(Args args)
    {
        _timelineManager.Pause();

        _spellCastQueue = args.Queue;
        ProcessNextSpellCast();
    }

    private void ProcessNextSpellCast()
    {
        var castInfo = _spellCastQueue.Dequeue();
        var spellCastResult = ProcessNewSpellCast(castInfo.Caster, castInfo.PreviousCastTime);
        ProcessSpellCastConsequences(castInfo.Caster, castInfo.PreviousCastTime, spellCastResult);
    }

    private void ProcessSpellCastConsequences(Actor caster, double previousCastTime,
        ActorController.SpellCastResult spellCastResult)
    {
        if (spellCastResult != ActorController.SpellCastResult.SpellInProcessOfCasting)
        {
            TryResume();
            return;
        }

        var spellUsageSubscription = _timelineManager
            .InitCastedSpellInfo.Subscribe(OnNextInitSpellCast);
        _actorSpellUsageSubscriptions.Add(caster, spellUsageSubscription);

        void OnNextInitSpellCast((ISpell Spell, SpellCastInfo CastInfo) value)
        {
            if (value.CastInfo.Caster != caster
                || value.CastInfo.InitialCastTime != previousCastTime)
            {
                return;
            }

            _actorSpellUsageSubscriptions[caster].Dispose();
            _actorSpellUsageSubscriptions.Remove(caster);
            TryResume();
        }
    }

    private void TryResume()
    {
        if (_actorsManager.UnitsQueueSize > 0
            || _actorSpellUsageSubscriptions.Count > 0)
        {
            ProcessNextSpellCast();
            return;
        }

        StateMachine.EnterState<IdleState>();
    }

    private ActorController.SpellCastResult ProcessNewSpellCast(Actor caster, double previousCastTime)
    {
        if (!caster.Spells.CanStartSpellCast())
        {
            return ActorController.SpellCastResult.None;
        }
        
        var actorController = _actorsManager.GetController(caster);
        return actorController.CastSpell(_actorsManager.GetOuterWorldInfo(caster, previousCastTime));
    }

    public override void Exit()
    {
        _spellCastQueue = null;
        _timelineManager.Resume();
    }

    public override void Dispose()
    {
        foreach (var disposable in _actorSpellUsageSubscriptions.Values)
        {
            disposable.Dispose();
        }
    }
}

internal class SpellCastQueue : Queue<(Actor Caster, double PreviousCastTime)>
{ }