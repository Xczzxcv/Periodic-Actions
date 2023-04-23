using Spells.Aspects;

namespace Spells
{
internal interface ISpellAspectsFactory
{
    ISpellAspect Create(SpellAspectConfig aspectConfig, string spellId);
}
}