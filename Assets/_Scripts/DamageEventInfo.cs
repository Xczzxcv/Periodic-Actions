using Actors;

internal readonly struct DamageEventInfo
{
    public readonly IActor DamageTarget;
    public readonly DamageInfo DamageInfo;
    public readonly double DealtDamageAmount;

    public DamageEventInfo(IActor damageTarget, DamageInfo damageInfo, double dealtDamageAmount)
    {
        DamageTarget = damageTarget;
        DamageInfo = damageInfo;
        DealtDamageAmount = dealtDamageAmount;
    }

    public override string ToString() => $"[DEI] Target: {DamageTarget}, Info: {DamageInfo}, " +
                                         $"Dmg: {DealtDamageAmount}";
}