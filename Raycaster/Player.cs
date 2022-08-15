using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Raycaster;

public class Player
{
    private const int RayCount = Maze.WindowWidth / Maze.WallStripWidth;
    private const float RotationSpeed = 45 * (MathHelper.Pi / 180);
    private const float MoveSpeed = 100f;
    private readonly IList<Ray> _rays = new List<Ray>(RayCount);

    public const float FovAngle = 60 * (MathHelper.Pi / 180);

    public IEnumerable<Ray> Rays => _rays;

    public float RotationAngle { get; set; }

    public Point2 Position { get; set; }

    public TurnDirection TurnDirection { get; set; }

    public WalkDirection WalkDirection { get; set; }

    public Player()
    {
        for (var i = 0; i < RayCount; i++)
        {
            _rays.Add(new Ray());
        }
    }

    public void Update(Maze maze, GameTime gameTime)
    {
        var deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        RotationAngle += (float)((int)TurnDirection * RotationSpeed * deltaTime);
        var moveStep = (float)((int)WalkDirection * MoveSpeed * deltaTime);
        var newX = Position.X + (float)Math.Cos(RotationAngle) * moveStep;
        var newY = Position.Y + (float)Math.Sin(RotationAngle) * moveStep;
        var newPosition = new Point2(newX, newY);

        if (!maze.HasWallAt(newPosition))
        {
            Position = newPosition;
        }
    }

    public void CastRays(Maze maze)
    {
        var rayAngle = RotationAngle - FovAngle / 2;
        foreach (var ray in _rays)
        {
            ray.Angle = MathUtils.NormalizeAngle(rayAngle);
            maze.CastRay(Position, ray);
            rayAngle += FovAngle / RayCount;
        }
    }
}
