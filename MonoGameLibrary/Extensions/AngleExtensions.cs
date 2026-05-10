using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Extensions;

public static class AngleExtensions
{
    /// <param name="angle"></param>
    extension(float angle)
    {
        /// <summary>
        /// Returns the wrapped angle which stays between 0 and 2PI
        /// </summary>
        /// <returns></returns>
        public float WrapAngle()
        {
            float twoPi = MathHelper.TwoPi;
            return (angle % twoPi + twoPi) % twoPi;
        }

        public float RotateTowards(
            float desiredAngle,
            float rotationSpeed,
            float deltaSeconds,
            out bool reached)
        {
            var current = angle.WrapAngle();
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

        public Vector2 ToUnitVector2()
        {
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }
    }
}