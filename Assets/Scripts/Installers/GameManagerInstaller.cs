using UnityEngine;
using Zenject;

namespace Installers
{
public class GameManagerInstaller : MonoInstaller
{
    [SerializeField] private GameManager gameManager;
    
    public override void InstallBindings()
    {
        Container.BindInstance(gameManager).AsSingle();
    }
}
}