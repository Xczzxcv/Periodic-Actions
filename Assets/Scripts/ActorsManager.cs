using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class ActorsManager : MonoBehaviour
{
    [SerializeField] private ActorController actorPrefab;
    [SerializeField] private Transform actorsParent;
    [SerializeField] private List<Transform> playerTeamPositions;
    [SerializeField] private List<Transform> enemyTeamPositions;

    private readonly HashSet<Vector3> _occupiedPositions = new();
    private readonly ActorControllersCollection _actorControllers = new();

    public void SpawnActor(Actor actor)
    {
        var spawnPos = GetSpawnPos(actor);
        var actorController = Instantiate(actorPrefab, spawnPos, Quaternion.identity, actorsParent);
        actorController.Init(_actorControllers);
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

        var positionTransform = posCollection.FirstOrDefault(
            posTransform => !_occupiedPositions.Contains(posTransform.position));
        
        if (positionTransform == null)
        {
            Debug.Log($"Can't find pos to spawn {actor}");
            return Vector3.zero;
        }

        return positionTransform.position;
    }
}
