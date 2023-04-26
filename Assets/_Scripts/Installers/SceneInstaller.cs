using Inventory;
using UnityEngine;
using Zenject;

namespace Installers
{
public class SceneInstaller : MonoInstaller
{
    [SerializeField] private PlayerInventoryController playerInventoryController;

    public override void InstallBindings()
    {
        BindTimelineManager();
        BindPlayerInventory();
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