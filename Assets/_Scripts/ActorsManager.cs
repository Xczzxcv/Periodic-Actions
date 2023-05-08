using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using Actors.Ai;
using Inventory;
using UnityEngine;
using Zenject;

internal partial class ActorsManager : MonoBehaviour, IDisposable
{
    [SerializeField] private ActorsSpawnManager actorsSpawnManager;
    [SerializeField] private List<ActorConfig> playerTeamConfigs;
    [SerializeField] private List<ActorConfig> enemyTeamConfigs;

    private TimelineManager _timelineManager;
    private PlayerInventory _playerInventory;
    private ActorsTeam _playerTeam;
    private ActorsTeam _enemyTeam;
    private ActorsSpellCastManager _actorsSpellCastManager;

    public int UnitsQueueSize => _actorsSpellCastManager.UnitsQueueSize;
    public ActorController PlayerActorController => actorsSpawnManager.GetController(_playerTeam.Actors.First());
    public ActorController EnemyActorController => actorsSpawnManager.GetController(_enemyTeam.Actors.First());

    [Inject]
    private void Construct(PlayerInventory playerInventory,
        TimelineManager timelineManager)
    {
        _playerInventory = playerInventory;
        _timelineManager = timelineManager;
    }

    public void Init()
    {
        _actorsSpellCastManager = new ActorsSpellCastManager(_timelineManager, this);

        _playerTeam = SpawnActorsTeam(playerTeamConfigs);
        _enemyTeam = SpawnActorsTeam(enemyTeamConfigs);

        Debug.Log("Init ended");

        InitSpellCasts();
    }

    private ActorsTeam SpawnActorsTeam(List<ActorConfig> configs)
    {
        var team = new ActorsTeam();
        foreach (var actorConfig in configs)
        {
            var actor = actorsSpawnManager.SpawnActor(actorConfig);
            team.AddMember(actor);
        }

        return team;
    }

    public void OnUpdate()
    {
        _actorsSpellCastManager.Update();
    }

    public bool IsGameEnded()
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

    public IActorAi.OuterWorldInfo GetOuterWorldInfo(IActor actor, int previousCastTime)
    {
        if (_playerTeam.ContainsMember(actor))
        {
            return new IActorAi.OuterWorldInfo
            {
                AllyTeam = _playerTeam,
                EnemyTeam = _enemyTeam,
                PreviousCastTime = previousCastTime
            };
        }

        if (_enemyTeam.ContainsMember(actor))
        {
            return new IActorAi.OuterWorldInfo
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
        _actorsSpellCastManager.TeamSpellCast(_playerTeam, initCastTime);
        _actorsSpellCastManager.TeamSpellCast(_enemyTeam, initCastTime);
    }

    public ActorController GetController(IActor actor)
    {
        return actorsSpawnManager.GetController(actor);
    }

    public void Dispose()
    {
        _actorsSpellCastManager.Dispose();
    }
}