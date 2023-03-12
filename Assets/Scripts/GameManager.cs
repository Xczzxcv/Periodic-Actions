using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using JetBrains.Annotations;
using Spells;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

class Team
{
    public IReadOnlyList<Actor> Actors=>_actors;
    private readonly List<Actor> _actors = new();

    public void AddMember(Actor.Config memberConfig)
    {
        
    }
}

internal class GameManager : MonoBehaviour
{
    [SerializeField] private ActorsManager actorsManager;

    [SerializeField] private List<Actor.Config> playerTeam;
    [SerializeField] private List<Actor.Config> enemyTeam;

    private TimelineManager _timelineManager;
    private TimeManager _timeManager;
    private List<Actor> _playerTeam;
    private List<Actor> _enemyTeam;
    private ActorsFactory _actorsFactory;

    private const int InitialCastTime = 0;

    [Inject]
    private void Construct(ActorsFactory actorsFactory, TimeManager timeManager)
    {
        _actorsFactory = actorsFactory;
        _timeManager = timeManager;
    }

    private void Start()
    {
        _timelineManager = new TimelineManager(_timeManager);

        _playerTeam = new List<Actor>();
        foreach (var actorConfig in playerTeam)
        {
            _playerTeam.Add(_actorsFactory.BuildActor(_timelineManager, actorConfig));
        }

        _enemyTeam = new List<Actor>();
        foreach (var actorConfig in enemyTeam)
        {
            _enemyTeam.Add(_actorsFactory.BuildActor(_timelineManager, actorConfig));
        }

        foreach (var actor in _playerTeam.Concat(_enemyTeam))
        {
            actorsManager.SpawnActor(actor);
        }

        Debug.Log("Init ended");

        SimulateTeamSpellCast(_playerTeam, _enemyTeam, InitialCastTime);
        SimulateTeamSpellCast(_enemyTeam, _playerTeam, InitialCastTime);
    }

    private void Update()
    {
        _timeManager.Update();

        while (_timelineManager.Update() == TimelineManager.UpdateResult.SpellProcessedAndCasted)
        {
            var deferredCastedSpellInfo = _timelineManager.MainCastedSpellInfo.Value;
            var castedSpellCaster = deferredCastedSpellInfo.CastInfo.Caster;
            var previousCastTime = deferredCastedSpellInfo.CastTime;
            SimulateSpellCast(castedSpellCaster, previousCastTime);
        }

        UpdateGameSpeed();
    }

    private void SimulateTeamSpellCast(List<Actor> castersTeam, List<Actor> targetTeam, double previousCastTime)
    {
        foreach (var caster in castersTeam)
        {
            SimulateSpellCast(caster, previousCastTime, targetTeam);
        }
    }

    private void SimulateSpellCast(Actor spellCaster, double previousCastTime, 
        [CanBeNull] List<Actor> targetTeam = null)
    {
        targetTeam ??= _playerTeam.Contains(spellCaster)
            ? _enemyTeam
            : _playerTeam;
        if (targetTeam.All(actor => !actor.CanBeTargeted()))
        {
            Debug.Break();
            Debug.Log("Team is dead....");
            return;
        }

        Actor randomEnemyActor;
        do
        {
            randomEnemyActor = targetTeam[Random.Range(0, targetTeam.Count)];
        } while (!randomEnemyActor.CanBeTargeted());

        if (spellCaster.CanCastSpells())
        {
            spellCaster.CastSpell(spellCaster.GetRandomSpellId(), 
                new SpellCastInfo(previousCastTime, spellCaster, randomEnemyActor));
        }
    }

    private void UpdateGameSpeed()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            _timeManager.SetGameSpeed(Math.Round(_timeManager.GameSpeed.Value / 2, 5));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            _timeManager.SetGameSpeed(Math.Round(_timeManager.GameSpeed.Value * 2, 5));
        }
    }
}
