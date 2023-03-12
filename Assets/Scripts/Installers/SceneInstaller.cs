using UnityEngine;
using Zenject;

namespace Installers
{
public class SceneInstaller : MonoInstaller
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUiController battleUiControllerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();
        
        Container.InstantiatePrefabForComponent<BattleUiController>(battleUiControllerPrefab);
    }
}
}