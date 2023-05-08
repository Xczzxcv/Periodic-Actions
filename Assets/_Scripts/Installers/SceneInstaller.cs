using Actors;
using Inventory;
using UnityEngine;
using Zenject;

namespace Installers
{
public class SceneInstaller : MonoInstaller
{
    [SerializeField] private ActorsFactory actorsFactory;
    [SerializeField] private PlayerInventoryController playerInventoryController;
    [SerializeField] private BallHitManager.Config ballHitManagerConfig;

    public override void InstallBindings()
    {
        BindActorsFactory();
        BindBallHitManager();
        BindTimelineManager();
        BindPlayerInventory();
    }

    private void BindActorsFactory()
    {
        Container.BindInterfacesTo<ActorsFactory>().FromInstance(actorsFactory).AsSingle();
    }

    private void BindBallHitManager()
    {
        Container.Bind<BallHitManager>()
            .AsSingle()
            .WithArguments(new object[] {ballHitManagerConfig});
    }

    private void BindTimelineManager()
    {
        Container.Bind<TimelineManager>().AsSingle();
    }

    private void BindPlayerInventory()
    {
        Container.BindInstance(playerInventoryController.GetInventory()).AsSingle();
    }
}
}