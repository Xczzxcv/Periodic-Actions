using System;
using UnityEngine;
using Zenject;

internal class GameManager : MonoBehaviour
{
    private TimeManager _timeManager;
    private TimelineManager _timelineManager;

    [Inject]
    private void Construct(TimeManager timeManager, TimelineManager timelineManager)
    {
        _timeManager = timeManager;
        _timelineManager = timelineManager;
    }

    private void Update()
    {
        _timeManager.Update();

        while (!_timelineManager.IsPaused 
               && _timelineManager.Update() == TimelineManager.UpdateResult.SpellProcessedAndCasted)
        { }

        UpdateGameSpeed();
    }

    private void UpdateGameSpeed()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            _timeManager.SetGameSpeed(Math.Round(_timeManager.GameSpeed.Value / 2, 5));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            _timeManager.SetGameSpeed(Math.Round(_timeManager.GameSpeed.Value * 2, 5));
        }
    }
}
