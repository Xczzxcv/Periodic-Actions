using Actors;

namespace Spells
{
internal interface ISpellsFactory
{
    ISpell Create(string spellId, Actor spellOwner);

    #region Debug

    SpellConfig[] GetAllSpellConfigs();

    #endregion
}
}