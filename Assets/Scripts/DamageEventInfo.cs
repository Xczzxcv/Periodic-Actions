using Actors;

internal readonly struct DamageEventInfo
{
    public readonly Actor DamageTarget;
    public readonly DamageInfo DamageInfo;
    public readonly float DealtDamageAmount;

    public DamageEventInfo(Actor damageTarget, DamageInfo damageInfo, float dealtDamageAmount)
    {
        DamageTarget = damageTarget;
        DamageInfo = damageInfo;
        DealtDamageAmount = dealtDamageAmount;
    }

    public override string ToString() => $"[DEI] Target: {DamageTarget}, Info: {DamageInfo}, " +
                                         $"Dmg: {DealtDamageAmount}";
}