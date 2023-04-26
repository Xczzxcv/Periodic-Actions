internal abstract class State<TObject> : StateBase<TObject>, IState<TObject>
{
    protected State(StateMachine<TObject> stateMachine) : base(stateMachine)
    { }

    public abstract void Enter();
}