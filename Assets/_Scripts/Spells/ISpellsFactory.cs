using Actors;

namespace Spells
{
internal interface ISpellsFactory
{
    ISpell Create(string spellId, IActor spellOwner);
}
}