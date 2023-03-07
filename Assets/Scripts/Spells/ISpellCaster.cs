namespace Spells
{
internal interface ISpellCaster
{
    public void ChangeArmor(float shiftAmount);
    public bool CanCastSpells();
    public SpellCastResult CastSpell(string spellId, SpellCastInfo castInfo);
    public bool IsPlayerUnit { get; }
    public bool IsAlive { get; }
    public bool HasSpell(string spellId);

    #region Debug

    public string GetRandomSpellId();

    #endregion
}
}