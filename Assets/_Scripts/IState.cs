using System;

internal interface IStateBase<TObject> : IDisposable
{
    void Exit();
}

internal interface IState<TObject> : IStateBase<TObject>
{
    void Enter();
}

internal interface IStateWithArgs<TObject, TArgs> : IStateBase<TObject>
    where TArgs : struct
{
    void Enter(TArgs args);
}