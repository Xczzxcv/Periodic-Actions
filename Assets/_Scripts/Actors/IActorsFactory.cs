using UnityEngine;

namespace Actors
{
internal interface IActorsFactory
{
    ActorController CreateActorController(Vector3 spawnPos, 
        ActorControllersCollection actorControllers);

    IActor CreateActor(TimelineManager timelineManager, ActorConfig config);
    AbsoluteActor CreateAbsoluteActor();
}
}