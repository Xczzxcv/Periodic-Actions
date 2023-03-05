internal interface ISpellCaster
{
    public void ChangeArmor(float shiftAmount);
    public bool CanCastSpells();
    public SpellCastResult CastSpell(string spellId, SpellCastInfo castInfo);
    bool IsPlayerUnit { get; }

    #region Debug

    public string GetRandomSpellId();

    #endregion
}