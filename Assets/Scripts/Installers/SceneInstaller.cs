using UnityEngine;
using Zenject;

namespace Installers
{
public class SceneInstaller : MonoInstaller
{
    [SerializeField] private BattleUiController battleUiControllerPrefab;
    [SerializeField] private RectTransform uiRoot;

    public override void InstallBindings()
    {
        BindTimelineManager();

        InstantiateBattleUiController();
    }

    private void BindTimelineManager()
    {
        Container.Bind<TimelineManager>().AsSingle();
    }

    private void InstantiateBattleUiController()
    {
        Container.InstantiatePrefab(battleUiControllerPrefab, uiRoot);
    }
}
}