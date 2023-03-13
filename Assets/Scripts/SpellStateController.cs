using System;
using JetBrains.Annotations;
using Spells;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

internal class SpellStateController : UIBehaviour, IDisposable
{
    [SerializeField] private Image progressBar;

    [CanBeNull] private IDisposable _subscription;
    private TimeManager _timeManager;
    private double _initCastSpellTime;
    private double _mainCastSpellTime;

    [Inject]
    private void Construct(TimeManager timeManager)
    {
        _timeManager = timeManager;
    }

    public void Setup([CanBeNull] ISpell spell, SpellCastInfo castInfo)
    {
        if (spell == null)
        {
            return;
        }

        _initCastSpellTime = castInfo.InitialCastTime;
        _mainCastSpellTime = _initCastSpellTime + spell.BaseConfig.Duration;
    }

    private void Update()
    {
        var currentTime = _timeManager.CurrentTime.Value;
        progressBar.fillAmount = Mathf.InverseLerp(
            (float) _initCastSpellTime,
            (float) _mainCastSpellTime,
            (float) currentTime
        );
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}