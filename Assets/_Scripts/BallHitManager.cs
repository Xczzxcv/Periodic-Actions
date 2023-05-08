using System;
using Actors;

internal class BallHitManager
{
    [Serializable]
    internal struct Config
    {
        public double DamageAmount;
        public bool DmgPierceArmor;
    }
    
    private readonly AbsoluteActor _absoluteActor;
    private readonly Config _config;

    public int BallHitCounter { get; private set; }
    public Action<int> BallHitCounterUpdated;

    public BallHitManager(IActorsFactory actorsFactory, Config config)
    {
        _absoluteActor = actorsFactory.CreateAbsoluteActor();
        _config = config;
    }

    public void HitBall(IActor hitter)
    {
        BallHitCounter++;
        hitter.ApplyDamage(new DamageInfo(
            _absoluteActor,
            _config.DamageAmount,
            _config.DmgPierceArmor,
            false
        ));

        BallHitCounterUpdated?.Invoke(BallHitCounter);
    }
}