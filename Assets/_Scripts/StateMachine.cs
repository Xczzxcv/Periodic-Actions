using System;
using System.Collections.Generic;

internal abstract class StateMachine<TObject> : IStateMachine<TObject>
{
    public IStateBase<TObject> CurrentState { get; private set; }

    protected readonly Dictionary<Type, IStateBase<TObject>> States = new();

    public void EnterState<TState>() where TState : IState<TObject>
    {
        var nextState = States[typeof(TState)] as IState<TObject>;
        
        CurrentState?.Exit();
        if (nextState == null)
        {
            throw new Exception($"WHYY??? {typeof(TObject)}, {typeof(TState)}");
        }

        CurrentState = nextState;
        nextState.Enter();
    }
    
    public void EnterStateWithArgs<TState, TArgs>(TArgs args) where TState : IStateWithArgs<TObject, TArgs> 
        where TArgs : struct
    {
        var nextState = States[typeof(TState)] as IStateWithArgs<TObject, TArgs>;
        
        CurrentState?.Exit();
        if (nextState == null)
        {
            throw new Exception($"WHYY??? {typeof(TObject)}, {typeof(TState)}, {typeof(TArgs)}");
        }

        CurrentState = nextState;
        nextState.Enter(args);
    }

    public bool IsInState<TState>() where TState : IStateBase<TObject>
    {
        return typeof(TState) == CurrentState.GetType();
    }

    public TState GetState<TState>() where TState : IStateBase<TObject>
    {
        return (TState) States[typeof(TState)];
    }

    public void Update()
    {
        if (CurrentState is IUpdatableState<TObject> updatableState)
        {
            updatableState.Update();
        }
    }

    public void Dispose()
    {
        foreach (var state in States.Values)
        {
            state.Dispose();
        }
    }
}