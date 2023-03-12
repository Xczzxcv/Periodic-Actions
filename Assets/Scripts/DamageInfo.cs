internal readonly struct DamageInfo
{
    public readonly Actor DamageSource;
    public readonly float DamageAmount;
    public readonly bool PierceArmor;
    public readonly bool ReturnedDamage;

    public DamageInfo(Actor damageSource, float damageAmount, bool pierceArmor,
        bool returnedDamage)
    {
        DamageSource = damageSource;
        DamageAmount = damageAmount;
        PierceArmor = pierceArmor;
        ReturnedDamage = returnedDamage;
    }
}