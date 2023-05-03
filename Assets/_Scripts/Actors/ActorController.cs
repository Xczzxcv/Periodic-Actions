using System;
using Actors.Ai;
using JetBrains.Annotations;
using Spells;
using UI;
using UniRx;
using UnityEngine;

namespace Actors
{
internal class ActorController : MonoBehaviour, IDisposable
{
    [SerializeField] private ActorView view;
    [SerializeField] private SpellChoiceController spellChoicePrefab;
    [SerializeField] private RectTransform spellChoiceParent;

    internal enum SpellCastResult
    {
        None,
        SpellCasted,
        SpellInProcessOfCasting
    }
    
    private Actor _actor;
    private IActorAi.OuterWorldInfo _outerWorldInfo;
    private SpellChoiceController _spellChoiceController;
    private ISpell _chosenSpell;

    public void Init(ActorControllersCollection actorControllers)
    {
        view.Init(actorControllers);
    }

    public void Setup(Actor actor)
    {
        _actor = actor;
        view.Setup(_actor);
    }

    public SpellCastResult CastSpell(IActorAi.OuterWorldInfo outerWorldInfo)
    {
        Debug.Assert(_actor.Spells.CanStartSpellCast(), $"{_actor} can't start cast spell");

        _outerWorldInfo = outerWorldInfo;

        if (_actor.IsPlayerUnit)
        {
            ShowSpellsToCast();
            return SpellCastResult.SpellInProcessOfCasting;
        }

        var spellCastChoice = _actor.GetAiSpellChoice(_outerWorldInfo);
        CastSpellInternal(_actor, spellCastChoice);
        return SpellCastResult.SpellCasted;
    }

    private static void CastSpellInternal(Actor actor, ActorSpellCastChoice spellChoice)
    {
        actor.Spells.CastSpell(spellChoice);
    }

    private void CastPlayerActorSpell([CanBeNull] Actor targetActor)
    {
        var spellCastChoice = ActorSpellCastChoice.Build(
            _chosenSpell.Id,
            _actor,
            targetActor,
            _outerWorldInfo.PreviousCastTime
        );

        view.SetHighlighted(false);
        CastSpellInternal(_actor, spellCastChoice);

        Destroy(_spellChoiceController.gameObject);
        _spellChoiceController = null;
    }

    private void ShowSpellsToCast()
    {
        _spellChoiceController = Instantiate(spellChoicePrefab, spellChoiceParent);
        _spellChoiceController.Setup(_actor.Spells.Spells.Values);
        _spellChoiceController.ChosenSpell.Subscribe(OnNextSpellChosen);
        view.SetHighlighted(true);
    }

    private void OnGUI()
    {
        if (!_spellChoiceController)
        {
            return;
        }
        
        if (_chosenSpell == null)
        {
            return;
        }

        if (_chosenSpell.CastTarget == SpellCastTarget.NoTarget)
        {
            return;
        }

        var buttonStartPoint = new Vector2(15, 100);
        var buttonSize = new Vector2(200, 70);
        var possibleTargets = _chosenSpell.CastTarget == SpellCastTarget.Ally
            ? _outerWorldInfo.AllyTeam.Actors
            : _outerWorldInfo.EnemyTeam.Actors;
        for (var i = 0; i < possibleTargets.Count; i++)
        {
            var targetActor = possibleTargets[i];
            var buttonPos = new Vector2(buttonStartPoint.x, buttonStartPoint.y + buttonSize.y * i);
            if (!GUI.Button(new Rect(buttonPos, buttonSize), targetActor.Name))
            {
                continue;
            }
            
            CastPlayerActorSpell(targetActor);
        }
    }

    private void OnNextSpellChosen(ISpell spell)
    {
        _chosenSpell = spell;

        if (spell.CastTarget == SpellCastTarget.NoTarget)
        {
            CastPlayerActorSpell(null);
        }
    }

    public void Dispose()
    {
        view.Dispose();
    }
}
}