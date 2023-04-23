using Actors;
using Actors.Stats;
using Spells;
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
        BindSpellAspectsFactory();
        BindSpellsFactory();
        BindStatsShiftFactory();
        BindActorsFactory();
        BindTimeManager();
    }

    private void BindSpellAspectsFactory()
    {
        Container.BindInterfacesTo<SpellAspectsFactory>().AsSingle();
    }

    private void BindSpellsFactory()
    {
        Container.BindInterfacesTo<SpellsFactory>().FromInstance(spellsFactory).AsSingle();
    }

    private void BindStatsShiftFactory()
    {
        Container.BindInterfacesTo<StatsShiftFactory>().AsSingle();
    }

    private void BindActorsFactory()
    {
        Container.BindInterfacesTo<ActorsFactory>().FromInstance(actorsFactory).AsSingle();
    }

    private void BindTimeManager()
    {
        Container.Bind<TimeManager>().AsSingle();
    }
}
}