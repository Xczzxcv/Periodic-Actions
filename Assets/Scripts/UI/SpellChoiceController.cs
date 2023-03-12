using System;
using System.Collections.Generic;
using System.Linq;
using Spells;
using UniRx;
using UnityEngine;

namespace UI
{
internal class SpellChoiceController : MonoBehaviour
{
    [SerializeField] private SpellChoiceOptionView spellOptionViewPrefab;
    [SerializeField] private Transform optionsParent;

    public IObservable<ISpell> ChosenSpell;

    private readonly List<SpellChoiceOptionView> _spellOptionViews = new();
    
    public void Setup(IEnumerable<ISpell> spells)
    {
        foreach (var spell in spells)
        {
            var spellChoiceOptionView = Instantiate(spellOptionViewPrefab, optionsParent);
            spellChoiceOptionView.Setup(spell);
            _spellOptionViews.Add(spellChoiceOptionView);
        }

        ChosenSpell = _spellOptionViews
            .Select(spellOptionView => spellOptionView.ChosenSpell)
            .Merge();
    }
}
}