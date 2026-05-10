using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Extensions;

public static class Vector2Extensions
{
    extension(Vector2 vector)
    {
        public float ToAngle()
        {
            return MathF.Atan2(vector.Y, vector.X);
        }

        public Vector2 GetRotatedVector(float rotation)
        {
            float currentAngle = vector.ToAngle();

            float newAngle = currentAngle + rotation;

            return newAngle.ToUnitVector2() * vector.Length();
        }
    }
}