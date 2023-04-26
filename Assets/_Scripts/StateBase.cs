internal abstract class StateBase<TObject> : IStateBase<TObject>
{
    protected StateMachine<TObject> StateMachine;

    protected StateBase(StateMachine<TObject> stateMachine)
    {
        StateMachine = stateMachine;
    }

    public abstract void Exit();

    public virtual void Dispose()
    { }
}