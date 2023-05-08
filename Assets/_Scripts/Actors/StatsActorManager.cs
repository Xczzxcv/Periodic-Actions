using System;
using System.Linq;
using Actors.Stats;
using UniRx;

namespace Actors
{
internal class StatsActorManager : ActorManager, IStatsProvider
{
    public IReadOnlyReactiveProperty<double> Hp => Get(ActorStat.Hp);
    public IReadOnlyReactiveProperty<double> MaxHp => Get(ActorStat.MaxHp);
    public IReadOnlyReactiveProperty<double> Armor => Get(ActorStat.Armor);

    private readonly IStatsShiftFactory _statsShiftFactory;
    private readonly ActorStatsCollection _stats = new();
    private readonly ReactiveCollection<IStatsShift> _statsShifts = new();
    private readonly ActorStatsCollection _complexStats = new();
    private readonly CompositeDisposable _compositeDisposable = new();

    public StatsActorManager(IStatsShiftFactory statsShiftFactory, IActor self) : base(self)
    {
        _statsShiftFactory = statsShiftFactory;
    }

    public override void Init(ActorConfig config)
    {
        UpdateAllStatValues();
        SubscribeOnAllStatUpdates();

        Set(ActorStat.MaxHp, config.Hp);
        Set(ActorStat.Hp, GetStat(ActorStat.MaxHp));
        Set(ActorStat.Armor, config.Armor);
    }

    public double GetStat(ActorStat stat) => Get(stat).Value;
    public IReadOnlyReactiveProperty<double> Get(ActorStat stat) => _complexStats[stat];

    public void Set(ActorStat stat, double newValue) => _stats[stat].Value = newValue;

    private void UpdateAllStatValues()
    {
        var statValues = Enum.GetValues(typeof(ActorStat));
        foreach (ActorStat stat in statValues)
        {
            _complexStats[stat].Value = GetComplexStatValue(stat);
        }
    }

    private void UpdateStatValue(ActorStat stat)
    {
        var complexStatValue = GetComplexStatValue(stat);
        _complexStats[stat].Value = complexStatValue;
    }

    private double GetComplexStatValue(ActorStat stat)
    {
        var actorStat = _stats.GetStat(stat);
        var statShifts = _statsShifts.Select(statsShift => statsShift.GetStat(stat));
        var statValues = Enumerable.Empty<double>()
            .Append(actorStat)
            .Concat(statShifts);
        var complexStatValue = statValues.Sum();
        return complexStatValue;
    }

    private void SubscribeOnAllStatUpdates()
    {
        var statValues = Enum.GetValues(typeof(ActorStat));
        foreach (ActorStat stat in statValues)
        {
            SubscribeOnStatUpdates(stat);
        }

        _statsShifts.ObserveAdd().Subscribe(_ => { UpdateAllStatValues(); }).AddTo(_compositeDisposable);
        _statsShifts.ObserveRemove().Subscribe(_ => { UpdateAllStatValues(); }).AddTo(_compositeDisposable);
        _statsShifts.ObserveReplace().Subscribe(_ => { UpdateAllStatValues(); }).AddTo(_compositeDisposable);
        
        
    }

    private void SubscribeOnStatUpdates(ActorStat stat)
    {
        var actorStat = _stats.Get(stat);
        var statShifts = _statsShifts.Select(statsShift => statsShift.Get(stat));
        var statProperties = Enumerable.Empty<IReadOnlyReactiveProperty<double>>()
            .Append(actorStat)
            .Concat(statShifts);
        var combineLatest = statProperties.CombineLatest();
        combineLatest
            .Select(statValues => statValues.Sum())
            .Subscribe(_ => OnStatUpdated(stat)).AddTo(_compositeDisposable);
    }

    private void OnStatUpdated(ActorStat updatedStat)
    {
        UpdateStatValue(updatedStat);
    }

    public void AddStatShift(StatsShiftConfig statsShiftConfig, string sourceId)
    {
        var statsShift = _statsShiftFactory.Create(new StatsShiftFactory.Args
        {
            statsShiftConfig = statsShiftConfig,
            owner = Self,
            SourceId = sourceId,
        });
        statsShift.Init();
        statsShift.StatShiftEnded += OnStatShiftEnded;
        _statsShifts.Add(statsShift);
    }

    private void OnStatShiftEnded(IStatsShift statsShift)
    {
        statsShift.StatShiftEnded -= OnStatShiftEnded;
        statsShift.Dispose();
        _statsShifts.Remove(statsShift);
    }
}
}