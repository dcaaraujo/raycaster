using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Raycaster;

public class Ray
{
    public float Angle { get; set; }
    public float Distance { get; set; }
    public Point2 WallIntersection { get; set; }
    public bool VerticalHit { get; set; }

    public bool IsFacingUp => !IsFacingDown;

    public bool IsFacingDown => Angle is > 0 and < MathHelper.Pi;

    public bool IsFacingLeft => !IsFacingRight;

    public bool IsFacingRight => Angle is < MathHelper.PiOver2 or > 1.5f * MathHelper.Pi;
}
