using UniRx;
using UnityEngine;

internal class TimeManager
{
    public IReadOnlyReactiveProperty<double> CurrentTime => _currentTime;
    public IReadOnlyReactiveProperty<double> GameSpeed => _gameSpeed;
    public IReadOnlyReactiveProperty<bool> IsPaused => _isPaused;

    private readonly ReactiveProperty<double> _currentTime;
    private readonly ReactiveProperty<double> _gameSpeed;
    private readonly ReactiveProperty<bool> _isPaused;
    private double _lastEngineTime;

    public TimeManager(double initialTime = 0)
    {
        _currentTime = new ReactiveProperty<double>(initialTime);
        _gameSpeed = new ReactiveProperty<double>(1);
        _isPaused = new ReactiveProperty<bool>(false);

        _gameSpeed.Subscribe(gameSpeed => Debug.Log($"New game speed value: {gameSpeed}"));
        _isPaused.Subscribe(isPaused => Debug.Log($"New is paused value: {isPaused}"));
    }

    public void Update()
    {
        var currentEngineTime = Time.timeAsDouble;
        var deltaTime = currentEngineTime - _lastEngineTime;
        _lastEngineTime = currentEngineTime;

        if (_isPaused.Value)
        {
            return;
        }

        _currentTime.Value += deltaTime * GameSpeed.Value;
    }

    public void SetGameSpeed(double gameSpeed)
    {
        _gameSpeed.Value = gameSpeed;
    }

    public void Pause()
    {
        _isPaused.Value = true;
    }

    public void Resume()
    {
        _isPaused.Value = false;
    }
}