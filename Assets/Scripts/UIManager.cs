using UnityEngine;
using Zenject;

internal class UIManager : MonoBehaviour
{
    private GameManager _manager;
    
    [Inject]
    public void Init(GameManager gameManager)
    {
        _manager = gameManager;
    }
}
