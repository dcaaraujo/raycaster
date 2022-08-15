using System;
using Microsoft.Xna.Framework;

namespace Raycaster;

public static class MathUtils
{
    public static double DistanceBetweenPoints(float x1, float y1, float x2, float y2)
    {
        return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
    }

    public static float NormalizeAngle(float angle)
    {
        angle %= 2 * MathHelper.Pi;
        if (angle >= 0)
        {
            return angle;
        }

        angle = MathHelper.TwoPi + angle;
        return angle;
    }
}
