using UnityEngine;
using Zenject;

internal class EntryPoint : MonoBehaviour
{
    [SerializeField] private ActorsManager actorsManager;
    [SerializeField] private BattleUiController battleUiControllerPrefab;
    [SerializeField] private BallController ballController;
    [SerializeField] private RectTransform uiRoot;

    private DiContainer _container;
    private BallHitManager _ballHitManager;

    [Inject]
    private void Construct(DiContainer container,
        BallHitManager ballHitManager)
    {
        _container = container;
        _ballHitManager = ballHitManager;
    }

    private void Start()
    {
        actorsManager.Init();
        ballController.Init(_ballHitManager, 
            actorsManager.PlayerActorController, actorsManager.EnemyActorController);
        InstantiateBattleUiController();
    }

    private void InstantiateBattleUiController()
    {
        _container.InstantiatePrefab(battleUiControllerPrefab, uiRoot);
    }
}
