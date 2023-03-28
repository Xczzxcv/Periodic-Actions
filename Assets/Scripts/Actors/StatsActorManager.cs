using UniRx;

namespace Actors
{
internal class StatsActorManager : ActorManager
{
    public IReadOnlyReactiveProperty<double> Hp => Get(Constants.Stats.HP);
    public IReadOnlyReactiveProperty<double> MaxHp => Get(Constants.Stats.MAX_HP);
    public IReadOnlyReactiveProperty<double> Armor => Get(Constants.Stats.ARMOR);

    private readonly ActorStatsCollection _stats = new();
    
    public StatsActorManager(Actor self) : base(self)
    { }

    public override void Init(Actor.Config config)
    {
        AddStat(Constants.Stats.HP, config.Hp);
        AddStat(Constants.Stats.MAX_HP, config.Hp);
        AddStat(Constants.Stats.ARMOR, config.Armor);
    }

    private void AddStat(string statId, double statValue)
    {
        _stats.Add(statId, new DoubleReactiveProperty(statValue));
    }
    
    public IReadOnlyReactiveProperty<double> Get(string statId) => _stats[statId];
    
    public void Set(string statId, double newValue) => _stats[statId].Value = newValue;
}
}