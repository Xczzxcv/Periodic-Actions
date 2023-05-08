using Actors;

internal readonly struct DamageInfo
{
    public readonly IActor DamageSource;
    public readonly double DamageAmount;
    public readonly bool PierceArmor;
    public readonly bool ReturnedDamage;

    public DamageInfo(IActor damageSource, double damageAmount, bool pierceArmor,
        bool returnedDamage)
    {
        DamageSource = damageSource;
        DamageAmount = damageAmount;
        PierceArmor = pierceArmor;
        ReturnedDamage = returnedDamage;
    }

    public override string ToString() => $"[DMG_INF] Src: {DamageSource}, Am: {DamageAmount}, " +
                                         $"PA: {PierceArmor}, RD: {ReturnedDamage}";
}