using JetBrains.Annotations;
using Spells;
using UnityEngine;

namespace UI
{
internal class SpellUiController : MonoBehaviour
{
    [SerializeField] private SpellView view;
    
    [CanBeNull] private ISpell _spell;

    public void Setup([CanBeNull] ISpell spell)
    {
        _spell = spell;
        
        view.Setup(new SpellView.Config
        {
            SpellName = spell?.Id,
            SpellProperties = spell?.GetProperties()
        });
    }
}
}