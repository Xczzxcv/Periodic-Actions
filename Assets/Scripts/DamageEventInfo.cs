using Actors;

internal readonly struct DamageEventInfo
{
    public readonly DamageInfo DamageInfo;
    public readonly Actor DamageTarget;

    public DamageEventInfo(DamageInfo damageInfo, Actor damageTarget)
    {
        DamageInfo = damageInfo;
        DamageTarget = damageTarget;
    }
}