using System;
using Actors.Ai;
using Spells;
using UI;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Actors
{
internal class ActorController : MonoBehaviour, IDisposable
{
    [SerializeField] private ActorView view;
    [SerializeField] private SpellChoiceController spellChoicePrefab;
    [SerializeField] private RectTransform spellChoiceParent;

    private Actor _actor;
    private ActorAiBase.OuterWorldInfo _outerWorldInfo;
    private SpellChoiceController _spellChoiceController;
    private Actor _chosenEnemyActor;

    public void Init(ActorControllersCollection actorControllers)
    {
        view.Init(actorControllers);
    }

    public void Setup(Actor actor)
    {
        _actor = actor;
        view.Setup(_actor);
    }

    public void CastSpell(ActorAiBase.OuterWorldInfo outerWorldInfo)
    {
        Debug.Assert(_actor.CanCastSpells());

        _outerWorldInfo = outerWorldInfo;

        if (_actor.IsPlayerUnit)
        {
            ShowSpellsToCast();
        }
        else
        {
            var spellCastChoice = _actor.GetAiSpellChoice(_outerWorldInfo);
            CastSpellInternal(_actor, spellCastChoice);
        }
    }

    private static void CastSpellInternal(Actor actor, ActorSpellCastChoice spellChoice)
    {
        actor.CastSpell(spellChoice);
    }

    private void ShowSpellsToCast()
    {
        _spellChoiceController = Instantiate(spellChoicePrefab, spellChoiceParent);
        _spellChoiceController.Setup(_actor.Spells.Values);
        _spellChoiceController.ChosenSpell.Subscribe(OnNextSpellChosen);
        view.SetHighlighted(true);
    }

    private void OnGUI()
    {
        if (!_spellChoiceController)
        {
            return;
        }

        var buttonStartPoint = new Vector2(15, 100);
        var buttonSize = new Vector2(200, 70);
        for (var i = 0; i < _outerWorldInfo.EnemyTeam.Actors.Count; i++)
        {
            var enemyActor = _outerWorldInfo.EnemyTeam.Actors[i];
            var buttonPos = new Vector2(buttonStartPoint.x, buttonStartPoint.y + buttonSize.y * i);
            if (GUI.Button(new Rect(buttonPos, buttonSize), enemyActor.Name))
            {
                _chosenEnemyActor = enemyActor;
            }
        }
    }

    private void OnNextSpellChosen(ISpell spell)
    {
        if (spell.IsTargeted && _chosenEnemyActor == null)
        {
            Debug.LogError($"You need to choose target to cast {spell}");
            return;
        }

        var spellCastInfo = new SpellCastInfo(
            _outerWorldInfo.PreviousCastTime,
            _actor,
            _chosenEnemyActor
        );
        var spellCastChoice = new ActorSpellCastChoice(
            spell.Id,
            spellCastInfo
        );

        view.SetHighlighted(false);
        Destroy(_spellChoiceController.gameObject);
        _spellChoiceController = null;
        CastSpellInternal(_actor, spellCastChoice);
    }

    public void Dispose()
    {
        view.Dispose();
    }
}
}