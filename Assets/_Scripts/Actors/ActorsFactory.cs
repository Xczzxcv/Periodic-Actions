using Actors.Ai;
using Actors.Stats;
using Spells;
using UnityEngine;
using Zenject;

namespace Actors
{
internal class ActorsFactory : MonoBehaviour, IActorsFactory
{
    [SerializeField] private ActorController actorPrefab;
    [SerializeField] private ActorConfig absoluteActorConfig;

    private ISpellsFactory _spellsFactory;
    private IStatsShiftFactory _statsShiftFactory;
    private IActorAiFactory _actorAiFactory;
    private DiContainer _diContainer;

    [Inject]
    private void Construct(
        ISpellsFactory spellsFactory,
        IStatsShiftFactory statsShiftFactory,
        IActorAiFactory actorAiFactory,
        DiContainer diContainer)
    {
        _spellsFactory = spellsFactory;
        _statsShiftFactory = statsShiftFactory;
        _actorAiFactory = actorAiFactory;
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

    public IActor CreateActor(TimelineManager timelineManager, ActorConfig config)
    {
        var newActor = new Actor(timelineManager, _spellsFactory, _statsShiftFactory, _actorAiFactory);
        newActor.Init(config);

        return newActor;
    }

    public AbsoluteActor CreateAbsoluteActor()
    {
        var newActor = new AbsoluteActor();
        newActor.Init(absoluteActorConfig);

        return newActor;
    }
}
}