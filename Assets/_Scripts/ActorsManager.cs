using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using Actors.Ai;
using Inventory;
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
    private PlayerInventory _playerInventory;
    private ActorsTeam _playerTeam;
    private ActorsTeam _enemyTeam;
    private IDisposable _spellPostCastSubscription;
    private readonly Dictionary<Actor, IDisposable> _actorSpellUsageSubscriptions = new();
    private readonly Queue<(Actor Caster, double PreviousCastTime)> _unitsWaitingList = new();

    private bool _currentlyCastingSpell;

    [Inject]
    private void Construct(PlayerInventory playerInventory,
        TimelineManager timelineManager)
    {
        _playerInventory = playerInventory;
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
        if (!caster.Spells.CanStartSpellCast())
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


    private string _inventorySlotIndex = "InventorySlotIndex";
    private string _actorName = "ActorName";

    private void OnGUI()
    {
        var textAreaSize = new Vector2(200, 50);
        const int indent = 20;
        var posX = Screen.width - textAreaSize.x - indent;
        var startPosY = Screen.height - (textAreaSize.y + indent) * 4;
        _inventorySlotIndex = GUI.TextField(
            new Rect(new Vector2(posX, startPosY), textAreaSize),
            _inventorySlotIndex
        );

        _actorName = GUI.TextField(
            new Rect(new Vector2(posX, startPosY + (textAreaSize.y + indent)), textAreaSize),
            _actorName
        );

        if (GUI.Button(
                new Rect(new Vector2(posX, startPosY + (textAreaSize.y + indent) * 2), textAreaSize),
                "Move item to actor"
            ))
        {
            ProcessItemToActorBtnClick();
            return;
        }

        if (GUI.Button(
                new Rect(new Vector2(posX, startPosY + (textAreaSize.y + indent) * 3), textAreaSize),
                "Move item from actor"
            ))
        {
            ProcessItemFromActorBtnClick();
            return;
        }
    }

    private void ProcessItemToActorBtnClick()
    {
        if (!int.TryParse(_inventorySlotIndex, out var itemIndex))
        {
            Debug.LogError($"Invalid inventory slot index '{_inventorySlotIndex}'");
            return;
        }

        var item = _playerInventory.CellsContent.ElementAt(itemIndex);
        var resultActor = Enumerable.Empty<Actor>()
            .Concat(_playerTeam.Actors)
            .Concat(_enemyTeam.Actors)
            .FirstOrDefault(actor => actor.Name == _actorName);

        if (resultActor == null)
        {
            Debug.LogError($"Has no actor with name {_actorName}");
            return;
        }

        if (resultActor.Inventory.EmptyCellsCount <= 0)
        {
            Debug.LogError($"{resultActor} has no empty slots in inventory");
            return;
        }

        var takenItem = _playerInventory.TakeAway(item);
        if (!resultActor.Inventory.TryPutInto(takenItem))
        {
            Debug.LogError($"We somehow can't put {takenItem} into {resultActor} inventory");
        }
    }
    
    private void ProcessItemFromActorBtnClick()
    {
        var resultActor = Enumerable.Empty<Actor>()
            .Concat(_playerTeam.Actors)
            .Concat(_enemyTeam.Actors)
            .FirstOrDefault(actor => actor.Name == _actorName);

        if (resultActor == null)
        {
            Debug.LogError($"Has no actor with name {_actorName}");
            return;
        }

        var firstActorItem = resultActor.Inventory.CellsContent.First();
        if (firstActorItem == null)
        {
            Debug.LogError($"{resultActor} has no item in first slot");
            return;
        }

        var takenItem = resultActor.Inventory.TakeAway(firstActorItem);
        if (!_playerInventory.TryPutInto(takenItem))
        {
            Debug.LogError($"We somehow can't put {takenItem} into player inventory inventory");
        }
    }
}