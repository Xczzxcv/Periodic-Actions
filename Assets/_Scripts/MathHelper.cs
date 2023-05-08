using System;

internal static class MathHelper
{
    public static double InverseLerp(double a, double b, double value)
    {
        return a != b
            ? Math.Clamp((value - a) / (b - a), 0, 1)
            : 0.0f;
    }
}