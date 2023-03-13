using System;
using System.Collections.Generic;
using Actors;
using Actors.Ai;
using Spells;
using UniRx;
using UnityEngine;
using Zenject;

internal class ActorsManager : MonoBehaviour, IDisposable
{
    [SerializeField] private ActorsSpawnManager actorsSpawnManager;
    [SerializeField] private List<Actor.Config> playerTeamConfigs;
    [SerializeField] private List<Actor.Config> enemyTeamConfigs;

    private TimelineManager _timelineManager;
    private ActorsTeam _playerTeam;
    private ActorsTeam _enemyTeam;
    private IDisposable _spellPostCastSubscription;
    private readonly Dictionary<Actor, IDisposable> _actorSpellUsageSubscriptions = new();
    private readonly Queue<(Actor Caster, double PreviousCastTime)> _unitsWaitingList = new();

    private bool _currentlyCastingSpell;

    [Inject]
    private void Construct(TimelineManager timelineManager)
    {
        _timelineManager = timelineManager;
    }

    private void Start()
    {
        _spellPostCastSubscription = _timelineManager.PostCastedSpellInfo.Subscribe(OnNextSpellPostCast);

        _playerTeam = SpawnActorsTeam(playerTeamConfigs);
        _enemyTeam = SpawnActorsTeam(enemyTeamConfigs);

        Debug.Log("Init ended");

        InitSpellCasts();
    }

    private ActorsTeam SpawnActorsTeam(List<Actor.Config> configs)
    {
        var team = new ActorsTeam();
        foreach (var actorConfig in configs)
        {
            var actor = actorsSpawnManager.SpawnActor(actorConfig);
            team.AddMember(actor);
        }

        return team;
    }

    private void OnNextSpellPostCast(DeferredSpellCastInfo deferredCastInfo)
    {
        var caster = deferredCastInfo.CastInfo.Caster;
        var previousCastTime = deferredCastInfo.CastTime;

        if (caster == null)
        {
            return;
        }

        if (IsGameEnded())
        {
            return;
        }

        AddToCastQueue(caster, previousCastTime);
    }

    private void AddToCastQueue(Actor caster, double previousCastTime)
    {
        _timelineManager.Pause();
        _unitsWaitingList.Enqueue((caster, previousCastTime));
    }

    private void Update()
    {
        if (_currentlyCastingSpell)
        {
            return;
        }

        if (!_unitsWaitingList.TryDequeue(out var castingInfo))
        {
            return;
        }

        var spellCastResult = ProcessNewSpellCast(castingInfo.Caster, castingInfo.PreviousCastTime);
        ProcessSpellCastConsequences(castingInfo.Caster, castingInfo.PreviousCastTime, spellCastResult);
    }

    private ActorController.SpellCastResult ProcessNewSpellCast(Actor caster, double previousCastTime)
    {
        if (!caster.CanStartSpellCast())
        {
            return ActorController.SpellCastResult.None;
        }
        
        var actorController = actorsSpawnManager.GetController(caster);
        return actorController.CastSpell(GetOuterWorldInfo(caster, previousCastTime));
    }

    private void ProcessSpellCastConsequences(Actor caster, double previousCastTime,
        ActorController.SpellCastResult spellCastResult)
    {
        if (spellCastResult != ActorController.SpellCastResult.SpellInProcessOfCasting)
        {
            ProcessResume();
            return;
        }

        _currentlyCastingSpell = true;
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
            ProcessResume();
        }
    }

    private void ProcessResume()
    {
        _currentlyCastingSpell = false;

        if (_unitsWaitingList.Count > 0 
            || _actorSpellUsageSubscriptions.Count > 0)
        {
            return;
        }

        _timelineManager.Resume();
    }

    private bool IsGameEnded()
    {
        if (!_playerTeam.IsAlive)
        {
            Debug.Log("GAME ENDED: Enemy wins!");
            Debug.Break();
            return true;
        }

        if (!_enemyTeam.IsAlive)
        {
            Debug.Log("GAME ENDED: Player wins!");
            Debug.Break();
            return true;
        }

        return false;
    }

    private ActorAiBase.OuterWorldInfo GetOuterWorldInfo(Actor actor, double previousCastTime)
    {
        if (_playerTeam.ContainsMember(actor))
        {
            return new ActorAiBase.OuterWorldInfo
            {
                AllyTeam = _playerTeam,
                EnemyTeam = _enemyTeam,
                PreviousCastTime = previousCastTime
            };
        }

        if (_enemyTeam.ContainsMember(actor))
        {
            return new ActorAiBase.OuterWorldInfo
            {
                AllyTeam = _enemyTeam,
                EnemyTeam = _playerTeam,
                PreviousCastTime = previousCastTime
            };
        }

        throw new ArgumentException($"Unknown actor {actor}");
    }

    private void InitSpellCasts()
    {
        const int initCastTime = 0;
        TeamSpellCast(_playerTeam, initCastTime);
        TeamSpellCast(_enemyTeam, initCastTime);
    }
    
    private void TeamSpellCast(ActorsTeam castersTeam, double castTime)
    {
        foreach (var caster in castersTeam.Actors)
        {
            AddToCastQueue(caster, castTime);
        }
    }
    
    public void Dispose()
    {
        _spellPostCastSubscription?.Dispose();
        foreach (var disposable in _actorSpellUsageSubscriptions.Values)
        {
            disposable.Dispose();
        }
    }
}