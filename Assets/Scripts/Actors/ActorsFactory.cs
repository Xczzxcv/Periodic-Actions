using System.Linq;
using UnityEngine;
using Zenject;

namespace Actors
{
internal class ActorsFactory : MonoBehaviour
{
    [SerializeField] private ActorController actorPrefab;
    
    private DiContainer _diContainer;

    [Inject]
    private void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    public ActorController BuildActorController(Vector3 spawnPos, 
        ActorControllersCollection actorControllers)
    {
        var actorController = _diContainer.InstantiatePrefabForComponent<ActorController>(
            actorPrefab, spawnPos, Quaternion.identity, null);
        actorController.Init(actorControllers);
        return actorController;
    }

    public Actor BuildActor(TimelineManager timelineManager, Actor.Config config)
    {
        var spellsFactory = _diContainer.Resolve<SpellsFactory>();
        var newActor = new Actor(timelineManager, spellsFactory);

        #region Debug

        config.SpellIds = spellsFactory._spellConfigsCollection.Keys.ToArray();

        #endregion

        newActor.Init(config);

        return newActor;
    }
}
}