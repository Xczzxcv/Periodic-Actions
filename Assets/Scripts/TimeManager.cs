using UniRx;
using UnityEngine;

internal class TimeManager
{
    public IReadOnlyReactiveProperty<double> CurrentTime => _currentTime;
    public IReadOnlyReactiveProperty<double> GameSpeed => _gameSpeed;

    private readonly ReactiveProperty<double> _currentTime;
    private readonly ReactiveProperty<double> _gameSpeed;
    private double _lastEngineTime;

    public TimeManager(double initialTime = 0)
    {
        _currentTime = new ReactiveProperty<double>(initialTime);
        _gameSpeed = new ReactiveProperty<double>(1);
    }

    public void Update()
    {
        var currentEngineTime = Time.timeAsDouble;
        var deltaTime = currentEngineTime - _lastEngineTime;

        _currentTime.Value += deltaTime * GameSpeed.Value;
        
        _lastEngineTime = currentEngineTime;
    }

    public void SetGameSpeed(double gameSpeed)
    {
        _gameSpeed.Value = gameSpeed;
    }
}