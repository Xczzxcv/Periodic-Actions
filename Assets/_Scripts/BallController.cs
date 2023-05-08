using Actors;
using UnityEngine;
using Zenject;

internal class BallController : MonoBehaviour
{
    [SerializeField] private float travelTime;
    [SerializeField] private AnimationCurve ballAnimationInfo;

    private TimeManager _timeManager;
    private BallHitManager _ballHitManager;
    private ActorController _playerActorController;
    private ActorController _enemyActorController;

    private double _startTravelTime;
    private ActorController _currentSrc;
    private ActorController _currentTarget;

    [Inject]
    private void Construct(TimeManager timeManager)
    {
        _timeManager = timeManager;
    }

    public void Init(BallHitManager ballHitManager, 
        ActorController playerActorController, ActorController enemyActorController)
    {
        _ballHitManager = ballHitManager;
        _playerActorController = playerActorController;
        _enemyActorController = enemyActorController;

        _currentSrc = _playerActorController;
        _currentTarget = _enemyActorController;
        transform.position = _currentSrc.GetBallHitPos();
        _startTravelTime = _timeManager.CurrentTime.Value;
    }

    private void Update()
    {
        if (_timeManager.IsPaused.Value)
        {
            return;
        }

        var timeParam = MathHelper.InverseLerp(
            _startTravelTime,
            _startTravelTime + travelTime,
            _timeManager.CurrentTime.Value
        );
        timeParam = ballAnimationInfo.Evaluate((float) timeParam);
        
        var srcPos = _currentSrc.GetBallHitPos();
        var targetPos = _currentTarget.GetBallHitPos();
        var nextPos = Vector3.Lerp(srcPos, targetPos, (float) timeParam);
        transform.position = nextPos;
        
        if (transform.position == targetPos)
        {
            _ballHitManager.HitBall(_currentTarget.Actor);
            (_currentSrc, _currentTarget) = (_currentTarget, _currentSrc);
            _startTravelTime = _timeManager.CurrentTime.Value;
        }
    }
}