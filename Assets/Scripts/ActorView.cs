using System;
using Actors;
using DG.Tweening;
using Spells;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

internal class ActorView : MonoBehaviour, IDisposable
{
    [SerializeField] private ActorTargetPointerController pointerController;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private SpellUiController castingSpell;
    [SerializeField] private SpellStateController spellStateController;
    [SerializeField] private Image isDeadIcon;
    [Header("Side")]
    [SerializeField] private Color playerSideColor;
    [SerializeField] private Color enemySideColor;
    [SerializeField] private Image sideIcon;
    [Header("Model")]
    [SerializeField] private SpriteRenderer model;
    [SerializeField] private Color regularColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private float highlightSwitchAnimDuration;

    private readonly CompositeDisposable _disposables = new();

    private Actor _actor;
    private ActorControllersCollection _actorControllers;

    public void Init(ActorControllersCollection actorControllers)
    {
        _actorControllers = actorControllers;
    }

    public void Setup(Actor actor)
    {
        Unsubscribe(_actor);
        _actor = actor;
        Subscribe(_actor);

        SetupPointerController(_actor);
        SetupName(_actor.Name);
        // SetupHp(_actor.Hp.Value);
        // SetupArmor(_actor.Armor.Value);
        // SetupCastingSpell(_actor.CastingSpell.Value);
        // SetupIsDead(_actor.IsDead.Value);
        // SetupSide(_actor.Side.Value);
    }

    private void Subscribe(Actor actor)
    {
        actor.Hp.Subscribe(SetupHp).AddTo(_disposables);
        actor.Armor.Subscribe(SetupArmor).AddTo(_disposables);
        actor.CastingSpell.Subscribe(SetupCastingSpell).AddTo(_disposables);
        actor.IsDead.Subscribe(SetupIsDead).AddTo(_disposables);
        actor.Side.Subscribe(SetupSide).AddTo(_disposables);
    }

    private void SetupName(string value) => nameText.text = value;

    private void SetupHp(float value) => hpText.text = $"{value}";
    
    private void SetupArmor(float value) => armorText.text = $"{value}";
    
    private void SetupCastingSpell((ISpell Spell, SpellCastInfo Castinfo) value)
    {
        spellStateController.Setup(value.Spell, value.Castinfo);
        castingSpell.Setup(value.Spell);
    }

    private void SetupIsDead(bool value) => isDeadIcon.enabled = value;

    private void SetupSide(ActorSide value)
    {
        sideIcon.color = value switch
        {
            ActorSide.Player => playerSideColor,
            ActorSide.Enemy => enemySideColor,
            _ => throw ThrowHelper.GetSideException(null, value),
        };
    }

    private void SetupPointerController(Actor actor)
    {
        pointerController.Setup(actor);

        actor.CastingSpell.Subscribe(CastingSpellHandler).AddTo(_disposables);
    }

    private void CastingSpellHandler((ISpell Spell, SpellCastInfo CastInfo) value)
    {
        if (value.Spell == null)
        {
            pointerController.RemovePointer();
            return;
        }

        if (!value.Spell.IsTargeted)
        {
            pointerController.RemovePointer();
            return;
        }

        var spellTarget = value.CastInfo.Target;
        if (_actorControllers.TryGetValue(spellTarget, out var actorController))
        {
            pointerController.PointTo(actorController);
        }
        else
        {
            Debug.LogError($"Can't point to {spellTarget}");
        }
    }

    private void Unsubscribe(Actor actor)
    {
        _disposables.Clear();
    }

    public void SetHighlighted(bool active)
    {
        var targetColor = active
            ? highlightedColor
            : regularColor;
        model.DOColor(targetColor, highlightSwitchAnimDuration);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}