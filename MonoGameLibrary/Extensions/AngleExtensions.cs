using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Extensions;

public static class AngleExtensions
{
    /// <summary>
    /// Returns the wrapped angle which stays between 0 and 2PI
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float WrapAngle(this float angle)
    {
        float twoPi = MathHelper.TwoPi;
        return (angle % twoPi + twoPi) % twoPi;
    }

    public static float RotateTowards(
        this float currentAngle,
        float desiredAngle,
        float rotationSpeed,
        float deltaSeconds,
        out bool reached)
    {
        var current = currentAngle.WrapAngle();
        var desired = desiredAngle.WrapAngle();

        var diff = desired - current;

        // now wrap diff to (-Pi, Pi)
        if (diff > MathHelper.Pi) diff -= MathHelper.TwoPi;
        if (diff < -MathHelper.Pi) diff += MathHelper.TwoPi;

        float maxStep = rotationSpeed * deltaSeconds;
        float newAngle;

        if (MathF.Abs(diff) <= maxStep)
        {
            newAngle = desiredAngle;
            reached = true;
        }
        else
        {
            newAngle = current + MathF.Sign(diff) * maxStep;
            reached = false;
        }

        return newAngle;
    }
}