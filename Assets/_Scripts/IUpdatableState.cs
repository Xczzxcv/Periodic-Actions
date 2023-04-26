internal interface IUpdatableState<TObject> : IStateBase<TObject>
{
    void Update();
}