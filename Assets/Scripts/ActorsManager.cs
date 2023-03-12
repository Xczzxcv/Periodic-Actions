using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using UnityEngine;
using Zenject;

internal class ActorsManager : MonoBehaviour, IDisposable
{
    [SerializeField] private List<Transform> playerTeamPositions;
    [SerializeField] private List<Transform> enemyTeamPositions;

    private readonly HashSet<Vector3> _occupiedPositions = new();
    private readonly ActorControllersCollection _actorControllers = new();

    private ActorsFactory _actorsFactory;

    [Inject]
    private void Construct(ActorsFactory actorsFactory)
    {
        _actorsFactory = actorsFactory;
    }

    public void SpawnActor(Actor actor)
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

    public void Dispose()
    {
        _actorControllers?.Dispose();
    }
}
