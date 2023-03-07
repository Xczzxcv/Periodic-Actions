using UnityEngine;

internal class ActorTargetPointerController : MonoBehaviour
{
    [SerializeField] private LineRenderer pointerLine;
    [SerializeField] private Transform playerSideSourcePos;
    [SerializeField] private Transform enemySideSourcePos;

    public void Setup(Actor actor)
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
        SetTargetPos(targetPos);
    }

    public void RemovePointer()
    {
        SetTargetPos(GetSourcePos());
    }

    private Vector3 GetSourcePos() => pointerLine.GetPosition(0);
    private void SetSourcePos(Vector3 position) => pointerLine.SetPosition(0, position);
    private void SetTargetPos(Vector3 position) => pointerLine.SetPosition(1, position);
}
