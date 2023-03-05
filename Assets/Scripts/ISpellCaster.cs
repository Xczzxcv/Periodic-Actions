public interface ISpellCaster
{
    public void ChangeArmor(float shiftAmount);
    public SpellCastResult CastSpell(string spellId, SpellCastInfo castInfo);

    #region Debug

    public string GetRandomSpellId();

    #endregion
}