using Inventory;
using UnityEngine;
using Zenject;

namespace Installers
{
public class SceneInstaller : MonoInstaller
{
    [SerializeField] private PlayerInventoryController playerInventoryController;
    [SerializeField] private BattleUiController battleUiControllerPrefab;
    [SerializeField] private RectTransform uiRoot;

    public override void InstallBindings()
    {
        BindTimelineManager();
        BindPlayerInventory();

        InstantiateBattleUiController();
    }

    private void BindTimelineManager()
    {
        Container.Bind<TimelineManager>().AsSingle();
    }

    private void BindPlayerInventory()
    {
        Container.BindInstance(playerInventoryController.GetInventory()).AsSingle();
    }

    private void InstantiateBattleUiController()
    {
        Container.InstantiatePrefab(battleUiControllerPrefab, uiRoot);
    }
}
}