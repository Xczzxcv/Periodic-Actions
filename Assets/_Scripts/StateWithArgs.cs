internal abstract class StateWithArgs<TObject, TArgs> : StateBase<TObject>, IStateWithArgs<TObject, TArgs> 
    where TArgs : struct
{
    protected StateWithArgs(StateMachine<TObject> stateMachine) : base(stateMachine)
    { }

    public abstract void Enter(TArgs args);
}