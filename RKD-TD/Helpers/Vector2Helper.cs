using System;
using Microsoft.Xna.Framework;

namespace RKD_TD.Helpers;

internal static class Vector2Helper
{
    public static Vector2 GetRotatedVector(
        Vector2 vector,
        float rotation)
    {
        float currentAngle = MathF.Atan2(vector.Y, vector.X);
        float newAngle = currentAngle + rotation;
        return new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle)) * vector.Length();
    }
}