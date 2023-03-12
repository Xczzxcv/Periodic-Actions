using Actors;
using UnityEngine;
using Zenject;

namespace Installers
{
public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private SpellsFactory spellsFactory;
    [SerializeField] private ActorsFactory actorsFactory;

    public override void InstallBindings()
    {
        BindSpellsFactory();
        BindActorsFactory();
        BindTimeManager();
    }

    private void BindActorsFactory()
    {
        Container.Bind<ActorsFactory>().FromInstance(actorsFactory).AsSingle();
    }

    private void BindSpellsFactory()
    {
        Container.Bind<SpellsFactory>().FromInstance(spellsFactory).AsSingle();
    }

    private void BindTimeManager()
    {
        Container.Bind<TimeManager>().AsSingle();
    }
}
}