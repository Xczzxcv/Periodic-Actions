using UnityEngine;
using Zenject;

internal class EntryPoint : MonoBehaviour
{
    [SerializeField] private ActorsManager actorsManager;
    [SerializeField] private BattleUiController battleUiControllerPrefab;
    [SerializeField] private RectTransform uiRoot;

    private DiContainer _container;

    [Inject]
    private void Construct(DiContainer container)
    {
        _container = container;
    }

    private void Start()
    {
        InstantiateBattleUiController();
        actorsManager.Init();
    }
    
    private void InstantiateBattleUiController()
    {
        _container.InstantiatePrefab(battleUiControllerPrefab, uiRoot);
    }
}
