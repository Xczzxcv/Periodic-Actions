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
        timeText.text = _timeManager.CurrentTime.Value.ToString("F1", CultureInfo.InvariantCulture);
        gameSpeedText.text = $"{_timeManager.GameSpeed.Value}";
    }
}