using System;
using Spells;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
internal class SpellChoiceOptionView : UIBehaviour
{
    [SerializeField] private SpellUiController spellViewController;
    [SerializeField] private Button confirmBtn;

    public IObservable<ISpell> ChosenSpell { get; private set; }

    private ISpell _spell;

    public void Setup(ISpell spell)
    {
        _spell = spell;
        spellViewController.Setup(spell);

        ChosenSpell = confirmBtn
            .OnClickAsObservable()
            .Select(_ => _spell);
        ChosenSpell.Subscribe(clickedSpell => Debug.Log($"{clickedSpell} was clicked"));
    }
}
}