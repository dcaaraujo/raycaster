using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Raycaster;

public class Maze
{
    public const int WindowWidth = ColumnCount * TileSize;
    public const int WindowHeight = RowCount * TileSize;
    public const int WallStripWidth = 8;
    public const int TileSize = 64;
    private const float MinimapScaleFactor = 0.2f;
    private const int RowCount = 11;
    private const int ColumnCount = 15;
    private const int PlayerDotRadius = 4;

    private readonly int[,] _grid =
    {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1 },
        { 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
    };

    public bool HasWallAt(Point2 position)
    {
        if (position.X is < 0 or > WindowWidth || position.Y is < 0 or > WindowHeight)
        {
            return true;
        }

        var gridX = (int)Math.Floor(position.X / TileSize);
        var gridY = (int)Math.Floor(position.Y / TileSize);
        return _grid[gridY, gridX] != 0;
    }

    private static (int, int) PositionOfPlayerInGrid(Player player)
    {
        var x = (int)Math.Floor(player.Position.X / TileSize);
        var y = (int)Math.Floor(player.Position.Y / TileSize);
        return (x, y);
    }

    public void CastRay(Point2 startPosition, Ray ray)
    {
        var horizontalPoint = CalculateHorizontalGridIntersection(startPosition, ray);
        var verticalPoint = CalculateVerticalGridIntersection(startPosition, ray);

        var horizontalHitDistance = horizontalPoint.HasValue
            ? MathUtils.DistanceBetweenPoints(startPosition.X, startPosition.Y, horizontalPoint.Value.X,
                horizontalPoint.Value.Y)
            : float.MaxValue;

        var verticalHitDistance = verticalPoint.HasValue
            ? MathUtils.DistanceBetweenPoints(startPosition.X, startPosition.Y, verticalPoint.Value.X,
                verticalPoint.Value.Y)
            : float.MaxValue;

        if (verticalHitDistance < horizontalHitDistance)
        {
            ray.Distance = (float)verticalHitDistance;
            ray.WallIntersection = verticalPoint ?? Point2.Zero;
            ray.VerticalHit = true;
        }
        else
        {
            ray.Distance = (float)horizontalHitDistance;
            ray.WallIntersection = horizontalPoint ?? Point2.Zero;
            ray.VerticalHit = false;
        }
    }

    private Point2? CalculateHorizontalGridIntersection(Point2 startPosition, Ray ray)
    {
        var yIntercept = (float)Math.Floor(startPosition.Y / TileSize) * TileSize;
        yIntercept += ray.IsFacingDown ? TileSize : 0;

        var xIntercept = startPosition.X + (yIntercept - startPosition.Y) / (float)Math.Tan(ray.Angle);
        float yStep = TileSize;
        yStep *= ray.IsFacingUp ? -1 : 1;

        var xStep = TileSize / (float)Math.Tan(ray.Angle);
        xStep *= ray.IsFacingLeft && xStep > 0 ? -1 : 1;
        xStep *= ray.IsFacingRight && xStep < 0 ? -1 : 1;

        var nextTouchX = xIntercept;
        var nextTouchY = yIntercept;

        // Increment xStep and yStep until we find a wall
        while (InsideMaze(new Point2(nextTouchX, nextTouchY)))
        {
            var xToCheck = nextTouchX;
            var yToCheck = nextTouchY + (ray.IsFacingUp ? -1 : 0);

            var testPoint = new Point2(xToCheck, yToCheck);
            if (HasWallAt(testPoint))
            {
                return testPoint;
            }

            nextTouchX += xStep;
            nextTouchY += yStep;
        }

        return null;
    }

    private Point2? CalculateVerticalGridIntersection(Point2 startPosition, Ray ray)
    {
        var xIntercept = (float)Math.Floor(startPosition.X / TileSize) * TileSize;
        xIntercept += ray.IsFacingRight ? TileSize : 0;

        var yIntercept = startPosition.Y + (xIntercept - startPosition.X) * (float)Math.Tan(ray.Angle);

        var xStep = TileSize;
        xStep *= ray.IsFacingLeft ? -1 : 1;

        var yStep = TileSize * (float)Math.Tan(ray.Angle);
        yStep *= ray.IsFacingUp && yStep > 0 ? -1 : 1;
        yStep *= ray.IsFacingDown && yStep < 0 ? -1 : 1;

        var nextTouchX = xIntercept;
        var nextTouchY = yIntercept;

        while (InsideMaze(new Point2(nextTouchX, nextTouchY)))
        {
            var xToCheck = nextTouchX + (ray.IsFacingLeft ? -1 : 0);
            var yToCheck = nextTouchY;

            var testPoint = new Point2(xToCheck, yToCheck);
            if (HasWallAt(testPoint))
            {
                return testPoint;
            }

            nextTouchX += xStep;
            nextTouchY += yStep;
        }

        return null;
    }

    private static bool InsideMaze(Point2 point)
    {
        return point.X is >= 0 and <= WindowWidth && point.Y is >= 0 and <= WindowHeight;
    }

    public void Draw(SpriteBatch spriteBatch, Player player)
    {
        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                float tileX = j * TileSize;
                float tileY = i * TileSize;
                var color = HasWallAt(new Point2(tileX, tileY)) ? Color.Gray : Color.White;
                var playerPosition = PositionOfPlayerInGrid(player);
                var isPlayerInBlock = playerPosition.Item1 == j && playerPosition.Item2 == i;
                if (isPlayerInBlock)
                {
                    color = Color.Yellow;
                }

                var rect = new RectangleF
                (
                    tileX * MinimapScaleFactor,
                    tileY * MinimapScaleFactor,
                    TileSize * MinimapScaleFactor,
                    TileSize * MinimapScaleFactor
                );
                spriteBatch.DrawRectangle(rect, Color.Black);
                spriteBatch.FillRectangle(rect, color);
            }
        }

        var playerDotPosition = new Point2
        (
            player.Position.X * MinimapScaleFactor,
            player.Position.Y * MinimapScaleFactor
        );
        var circle = new CircleF(playerDotPosition, PlayerDotRadius);
        spriteBatch.DrawCircle(circle, 20, Color.Black, 2f);
        foreach (var ray in player.Rays)
        {
            spriteBatch.DrawLine(playerDotPosition, ray.Distance * Maze.MinimapScaleFactor, ray.Angle, Color.Red);
        }
    }
}
