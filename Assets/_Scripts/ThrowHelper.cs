using System;
using Actors;
using JetBrains.Annotations;

internal static class ThrowHelper
{
    public static Exception WrongConfigTypeInFactory<TConfig, TResult>(TConfig config)
    {
        return new ArgumentException($"Unsupported config type in factory of {typeof(TResult)} ({config?.GetType()}) {config}");
    }
    
    public static Exception GetSideException([CanBeNull] Actor actor, ActorSide side)
    {
        return new ArgumentException($"Unknown {actor} side {side}");
    }
}