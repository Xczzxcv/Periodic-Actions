using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using Actors.Ai;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

internal class ActorsManager : MonoBehaviour, IDisposable
{
    [FormerlySerializedAs("playerTeam")]
    [SerializeField] private List<Actor.Config> playerTeamConfigs;
    [FormerlySerializedAs("enemyTeam")]
    [SerializeField] private List<Actor.Config> enemyTeamConfigs;
    [SerializeField] private List<Transform> playerTeamPositions;
    [SerializeField] private List<Transform> enemyTeamPositions;

    private readonly HashSet<Vector3> _occupiedPositions = new();
    private readonly ActorControllersCollection _actorControllers = new();

    private TimelineManager _timelineManager;
    private ActorsFactory _actorsFactory;
    private ActorsTeam _playerTeam;
    private ActorsTeam _enemyTeam;
    private IDisposable _spellPostCastSubscription;
    private readonly Dictionary<Actor, IDisposable> _actorSpellUsageSubscriptions = new();
    private readonly Queue<(Actor Caster, double PreviousCastTime)> _unitsWaitingList = new();

    private bool CurrentlyCastingSpell => _timelineManager.IsPaused;

    [Inject]
    private void Construct(TimelineManager timelineManager, ActorsFactory actorsFactory)
    {
        _timelineManager = timelineManager;
        _actorsFactory = actorsFactory;
    }

    private void Start()
    {
        _spellPostCastSubscription = _timelineManager.PostCastedSpellInfo.Subscribe(OnNextSpellPostCast);
        
        _playerTeam = new ();
        foreach (var actorConfig in playerTeamConfigs)
        {
            _playerTeam.AddMember(_actorsFactory.BuildActor(_timelineManager, actorConfig));
        }

        _enemyTeam = new ();
        foreach (var actorConfig in enemyTeamConfigs)
        {
            _enemyTeam.AddMember(_actorsFactory.BuildActor(_timelineManager, actorConfig));
        }

        foreach (var actor in _playerTeam.Actors.Concat(_enemyTeam.Actors))
        {
            SpawnActor(actor);
        }
        
        Debug.Log("Init ended");

        InitSpellCasts();
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

        _unitsWaitingList.Enqueue((caster, previousCastTime));
    }

    private void Update()
    {
        if (CurrentlyCastingSpell)
        {
            return;
        }

        if (!_unitsWaitingList.TryDequeue(out var castingInfo))
        {
            return;
        }

        ProcessNewSpellCast(castingInfo.Caster, castingInfo.PreviousCastTime);
    }

    private void ProcessNewSpellCast(Actor caster, double previousCastTime)
    {
        if (!caster.CanCastSpells())
        {
            return;
        }
        
        var actorController = _actorControllers[caster];
        actorController.CastSpell(GetInfo(caster, previousCastTime));

        if (!caster.IsPlayerUnit)
        {
            return;
        }

        _timelineManager.Pause();
        var spellUsageSubscription = _timelineManager.InitCastedSpellInfo.Subscribe(value =>
        {
            if (value.CastInfo.Caster != caster || value.CastInfo.InitialCastTime != previousCastTime)
            {
                return;
            }

            _actorSpellUsageSubscriptions[caster].Dispose();
            _actorSpellUsageSubscriptions.Remove(caster);
            if (_actorSpellUsageSubscriptions.Count <= 0)
            {
                _timelineManager.Resume();
            }
        });

        _actorSpellUsageSubscriptions.Add(caster, spellUsageSubscription);
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

    private ActorAiBase.OuterWorldInfo GetInfo(Actor actor, double previousCastTime)
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

    private void SpawnActor(Actor actor)
    {
        var spawnPos = GetSpawnPos(actor);
        var actorController = _actorsFactory.BuildActorController(spawnPos, _actorControllers);
        actorController.Setup(actor);

        _occupiedPositions.Add(spawnPos);
        _actorControllers.Add(actor, actorController);
    }

    private Vector3 GetSpawnPos(Actor actor)
    {
        var posCollection = actor.Side.Value switch
        {
            ActorSide.Player => playerTeamPositions,
            ActorSide.Enemy => enemyTeamPositions,
            _ => throw ThrowHelper.GetSideException(actor, actor.Side.Value)
        };

        var positionTransform = posCollection.FirstOrDefault(posTransform => 
            !_occupiedPositions.Contains(posTransform.position));
        
        if (positionTransform == null)
        {
            Debug.Log($"Can't find pos to spawn {actor}");
            return Vector3.zero;
        }

        return positionTransform.position;
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
            _unitsWaitingList.Enqueue((caster, castTime));
        }
    }
    
    public void Dispose()
    {
        _spellPostCastSubscription?.Dispose();
        foreach (var disposable in _actorSpellUsageSubscriptions.Values)
        {
            disposable.Dispose();
        }

        _actorControllers?.Dispose();
    }
}