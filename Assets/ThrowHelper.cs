using System;
using JetBrains.Annotations;

internal static class ThrowHelper
{
    public static Exception GetSideException([CanBeNull] Actor actor, ActorSide side)
    {
        return new ArgumentException($"Unknown {actor} side {side}");
    }
}