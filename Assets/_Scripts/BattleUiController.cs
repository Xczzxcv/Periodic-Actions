using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

internal class BattleUiController : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI gameSpeedText;

    private TimeManager _timeManager;

    [Inject]
    private void Construct(TimeManager timeManager)
    {
        _timeManager = timeManager;
    }

    private void Update()
    {
        timeText.text = TextHelper.Format(_timeManager.CurrentTime.Value);
        var gameSpeedView = _timeManager.GameSpeed.Value >= 1
            ? _timeManager.GameSpeed.Value.ToString(CultureInfo.InvariantCulture)
            : $"1/{(1 / _timeManager.GameSpeed.Value).ToString(CultureInfo.InvariantCulture)}";
        gameSpeedText.text = $"X{gameSpeedView}";
    }
}