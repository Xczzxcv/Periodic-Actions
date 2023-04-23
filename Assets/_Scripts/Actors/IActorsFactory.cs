using UnityEngine;

namespace Actors
{
internal interface IActorsFactory
{
    ActorController CreateActorController(Vector3 spawnPos, 
        ActorControllersCollection actorControllers);

    Actor CreateActor(TimelineManager timelineManager, Actor.Config config);
}
}