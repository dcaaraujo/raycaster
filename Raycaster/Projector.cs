using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Raycaster;

public class Projector
{
    public void Draw(SpriteBatch spriteBatch, Player player)
    {
        var ceilingRect = new RectangleF(0, 0, Maze.WindowWidth, Maze.WindowHeight / 2f);
        spriteBatch.FillRectangle(ceilingRect, new Color(0.25f, 0.25f, 0.25f));

        var floorRect = new RectangleF(0, Maze.WindowHeight / 2f, Maze.WindowWidth, Maze.WindowHeight);
        spriteBatch.FillRectangle(floorRect, new Color(0.51f, 0.51f, 0.51f));
        
        foreach (var (i, ray) in player.Rays.Select((r, i) => (i, r)))
        {
            var wallDistance = ray.Distance * Math.Cos(ray.Angle - player.RotationAngle);
            var distanceProjectionPlane = Maze.WindowWidth / 2f / Math.Tan(Player.FovAngle / 2);

            var wallStripHeight = Maze.TileSize / wallDistance * distanceProjectionPlane;

            var color = ray.VerticalHit ? Color.White : new Color(0.78f, 0.78f, 0.78f);

            var wallRect = new RectangleF
            (
                i * Maze.WallStripWidth,
                (float)(Maze.WindowHeight / 2.0 - wallStripHeight / 2.0),
                Maze.WallStripWidth,
                (float)wallStripHeight
            );
            spriteBatch.FillRectangle(wallRect, color);
        }
    }
}
