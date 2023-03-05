using System.Collections.Generic;
using JetBrains.Annotations;
using SpellConfigs;
using UnityEngine;
using Random = UnityEngine.Random;

internal class GameManager : MonoBehaviour
{
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
    private List<SpellConfigBase> spellConfigs; 

    [SerializeField] private List<Actor.Config> playerTeam; 
    [SerializeField] private List<Actor.Config> enemyTeam; 
    
    private TimelineManager _timelineManager;
    private TimeManager _timeManager;
    private int _spellCastSimulated;
    private List<Actor> _playerTeam;
    private List<Actor> _enemyTeam;

    private const int SpellCastSimulationsAmount = 10;
    private const int InitialCastTime = 0;

    private void Start()
    {
        _timeManager = new TimeManager();
        _timelineManager = new TimelineManager(_timeManager);

        var spells = new ISpell[spellConfigs.Count];
        for (var i = 0; i < spellConfigs.Count; i++)
        {
            var spellConfig = spellConfigs[i];
            var spell = EntitiesFactory.BuildSpell(spellConfig);
            spells[i] = spell;
        }

        _playerTeam = new List<Actor>();
        foreach (var actorConfig in playerTeam)
        {
            _playerTeam.Add(Actor.Build(_timelineManager, actorConfig, spells));
        }

        _enemyTeam = new List<Actor>();
        foreach (var actorConfig in enemyTeam)
        {
            _enemyTeam.Add(Actor.Build(_timelineManager, actorConfig, spells));
        }

        Debug.Log("Init ended");

        SimulateTeamSpellCast(_playerTeam, _enemyTeam, InitialCastTime);
        SimulateTeamSpellCast(_enemyTeam, _playerTeam, InitialCastTime);
    }

    private void Update()
    {
        _timeManager.Update();
        while (_timelineManager.Update())
        {
            var deferredCastedSpellInfo = _timelineManager.CastedSpellInfo.Value;
            var castedSpellCaster = deferredCastedSpellInfo.CastInfo.Caster;
            var previousCastTime = deferredCastedSpellInfo.CastTime;
            SimulateSpellCast(castedSpellCaster, previousCastTime);
        }

        if (_spellCastSimulated >= SpellCastSimulationsAmount)
        {
            Debug.Log("Simulation stopped!!!!!!");
            Debug.Break();
        }
    }

    private void SimulateTeamSpellCast(List<Actor> castersTeam, List<Actor> targetTeam, double previousCastTime)
    {
        foreach (var caster in castersTeam)
        {
            SimulateSpellCast(caster, previousCastTime, targetTeam);
        }
    }

    private void SimulateSpellCast(ISpellCaster spellCaster, double previousCastTime, [CanBeNull] List<Actor> targetTeam = null)
    {
        targetTeam ??= _playerTeam.Contains((Actor) spellCaster)
            ? _enemyTeam
            : _playerTeam;
        var randomEnemyActor = targetTeam[Random.Range(0, targetTeam.Count)];

        spellCaster.CastSpell(spellCaster.GetRandomSpellId(), new SpellCastInfo
        {
            PreCastTime = previousCastTime,
            Caster = spellCaster,
            Target = randomEnemyActor,
        });
        _spellCastSimulated++;
    }
}
