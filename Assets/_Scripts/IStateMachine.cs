using System;

internal interface IStateMachine<TObject> : IDisposable
{
    IStateBase<TObject> CurrentState { get; }
    void EnterState<TState>() where TState : IState<TObject>;
    void EnterStateWithArgs<TState, TArgs>(TArgs args) where TState : IStateWithArgs<TObject, TArgs> 
        where TArgs : struct;
    bool IsInState<TState>() where TState : IStateBase<TObject>;
    TState GetState<TState>() where TState : IStateBase<TObject>;
    void Update();
}