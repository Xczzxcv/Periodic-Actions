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

    private IActorAi.OuterWorldInfo _outerWorldInfo;
    private SpellChoiceController _spellChoiceController;
    private ISpell _chosenSpell;
    
    public IActor Actor { get; private set; }

    public void Init(ActorControllersCollection actorControllers)
    {
        view.Init(actorControllers);
    }

    public void Setup(IActor actor)
    {
        Actor = actor;
        view.Setup(Actor);
    }

    public SpellCastResult CastSpell(IActorAi.OuterWorldInfo outerWorldInfo)
    {
        Debug.Assert(Actor.Spells.CanStartSpellCast(), $"{Actor} can't start cast spell");

        _outerWorldInfo = outerWorldInfo;

        if (Actor.IsPlayerUnit)
        {
            ShowSpellsToCast();
            return SpellCastResult.SpellInProcessOfCasting;
        }

        var spellCastChoice = Actor.Ai.ChooseSpell(_outerWorldInfo);
        CastSpellInternal(Actor, spellCastChoice);
        return SpellCastResult.SpellCasted;
    }

    private static void CastSpellInternal(IActor actor, ActorSpellCastChoice spellChoice)
    {
        actor.Spells.CastSpell(spellChoice);
    }

    private void CastPlayerActorSpell([CanBeNull] IActor targetActor)
    {
        var spellCastChoice = ActorSpellCastChoice.Build(
            _chosenSpell.Id,
            Actor,
            targetActor,
            _outerWorldInfo.PreviousCastTime
        );

        view.SetHighlighted(false);
        CastSpellInternal(Actor, spellCastChoice);

        Destroy(_spellChoiceController.gameObject);
        _spellChoiceController = null;
    }

    private void ShowSpellsToCast()
    {
        _spellChoiceController = Instantiate(spellChoicePrefab, spellChoiceParent);
        _spellChoiceController.Setup(Actor.Spells.Spells.Values);
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

    public Vector3 GetBallHitPos()
    {
        return view.GetBallHitPos();
    }

    public void Dispose()
    {
        view.Dispose();
    }
}
}