using System;
using System.Collections;
using System.Collections.Generic;
using Actors;
using UnityEngine;
using Zenject;

internal class ActorTargetPointerController : MonoBehaviour
{
    [SerializeField] private LineRenderer pointerLine;
    [SerializeField] private Transform playerSideSourcePos;
    [SerializeField] private Transform enemySideSourcePos;
    [Header("Animation")]
    [SerializeField] private double regularAnimationDuration;
    [SerializeField] private double fastAnimationDuration;

    private TimeManager _timeManager;
    private readonly Queue<(Vector3, double)> _targets = new();
    private bool _settingTarget;

    [Inject]
    private void Construct(TimeManager timeManager)
    {
        _timeManager = timeManager;
    }

    public void Setup(IActor actor)
    {
        var side = actor.Side.Value;
        var sourcePos = side switch
        {
            ActorSide.Player => playerSideSourcePos,
            ActorSide.Enemy => enemySideSourcePos,
            _ => throw ThrowHelper.GetSideException(actor, side)
        };
        SetSourcePos(sourcePos.position);
    }

    public void PointTo(ActorController targetActor)
    {
        var targetPos = targetActor.transform.position;
        _targets.Enqueue((targetPos, regularAnimationDuration));
    }

    public void RemovePointer()
    {
        _targets.Enqueue((GetSourcePos(), fastAnimationDuration));
    }

    private void Update()
    {
        if (_settingTarget)
        {
            return;
        }

        if (!_targets.TryDequeue(out var targetInfo))
        {
            return;
        }

        StartCoroutine(SetTargetCoroutine(targetInfo.Item1, targetInfo.Item2));
    }

    private IEnumerator SetTargetCoroutine(Vector3 targetPos, double animationDuration)
    {
        _settingTarget = true;
        
        var animationStartTime = _timeManager.CurrentTime.Value;
        var animationEndTime = animationStartTime + animationDuration;
        var startTargetPos = GetTargetPos();
        do
        {
            yield return null;
            var timeParam = Mathf.InverseLerp(
                (float) animationStartTime,
                (float) animationEndTime,
                (float) _timeManager.CurrentTime.Value
            );
            var easedTimeParam = Ease(timeParam);
            SetTargetPos(Vector3.Lerp(startTargetPos, targetPos, easedTimeParam));
        } while (targetPos != GetTargetPos());

        _settingTarget = false;
    }

    private static float Ease(float timeParam)
    {
        return timeParam == 1
            ? 1
            : 1 - (float) Math.Pow(2, -10 * timeParam);
    }

    private Vector3 GetSourcePos() => pointerLine.GetPosition(0);
    private Vector3 GetTargetPos() => pointerLine.GetPosition(1);
    private void SetSourcePos(Vector3 position) => pointerLine.SetPosition(0, position);
    private void SetTargetPos(Vector3 position) => pointerLine.SetPosition(1, position);
}
