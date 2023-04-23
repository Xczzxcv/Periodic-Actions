using Actors.Stats;
using Spells;
using UnityEngine;
using Zenject;

namespace Actors
{
internal class ActorsFactory : MonoBehaviour, IActorsFactory
{
    [SerializeField] private ActorController actorPrefab;

    private ISpellsFactory _spellsFactory;
    private IStatsShiftFactory _statsShiftFactory;
    private DiContainer _diContainer;

    [Inject]
    private void Construct(
        ISpellsFactory spellsFactory,
        IStatsShiftFactory statsShiftFactory,
        DiContainer diContainer)
    {
        _spellsFactory = spellsFactory;
        _statsShiftFactory = statsShiftFactory;
        _diContainer = diContainer;
    }

    public ActorController CreateActorController(Vector3 spawnPos, 
        ActorControllersCollection actorControllers)
    {
        var actorController = _diContainer.InstantiatePrefabForComponent<ActorController>(
            actorPrefab, spawnPos, Quaternion.identity, null);
        actorController.Init(actorControllers);
        return actorController;
    }

    public Actor CreateActor(TimelineManager timelineManager, Actor.Config config)
    {
        var newActor = new Actor(timelineManager, _spellsFactory, _statsShiftFactory);

        #region Debug

        // config.SpellConfigs = _spellsFactory.GetAllSpellConfigs();

        #endregion

        newActor.Init(config);

        return newActor;
    }
}
}