public interface ISpellTarget
{
    public bool CanBeTargeted();
    public void ApplyDamage(float damageAmount, bool pierceArmor = false);
}