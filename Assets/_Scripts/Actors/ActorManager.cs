using System;

namespace Actors
{
internal abstract class ActorManager : IDisposable
{
    protected readonly IActor Self;

    protected ActorManager(IActor self)
    {
        Self = self;
    }

    public abstract void Init(ActorConfig config);

    public virtual void Dispose()
    { }
}
}