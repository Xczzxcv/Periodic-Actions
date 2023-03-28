using System;

namespace Actors
{
internal abstract class ActorManager : IDisposable
{
    protected readonly Actor Self;

    protected ActorManager(Actor self)
    {
        Self = self;
    }

    public abstract void Init(Actor.Config config);

    public virtual void Dispose()
    { }
}
}