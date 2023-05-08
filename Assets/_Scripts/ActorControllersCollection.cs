using System;
using System.Collections.Generic;
using Actors;

internal class ActorControllersCollection : Dictionary<IActor, ActorController>, IDisposable
{
    public void Dispose()
    {
        foreach (var actorController in Values)
        {
            actorController.Dispose();
        }
    }
}