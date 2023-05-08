using System;
using JetBrains.Annotations;
using Spells;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

internal class SpellStateController : UIBehaviour, IDisposable
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI countdownText;

    private TimeManager _timeManager;
    private BallHitManager _ballHitManager;

    private int _initCastSpellTime;
    private int _mainCastSpellTime;
    [CanBeNull] private IDisposable _subscription;

    [Inject]
    private void Construct(TimeManager timeManager, 
        BallHitManager ballHitManager)
    {
        _timeManager = timeManager;
        _ballHitManager = ballHitManager;
    }

    public void Setup([CanBeNull] ISpell spell, SpellCastInfo castInfo)
    {
        if (spell == null)
        {
            return;
        }

        _initCastSpellTime = castInfo.InitialCastTime;
        _mainCastSpellTime = _initCastSpellTime + spell.GetDelay();
    }

    private void Update()
    {
        var currentTime = _ballHitManager.BallHitCounter;
        progressBar.fillAmount = Mathf.InverseLerp(
            _initCastSpellTime,
            _mainCastSpellTime,
            currentTime
        );
        countdownText.text = TextHelper.Format(_mainCastSpellTime - currentTime);
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}