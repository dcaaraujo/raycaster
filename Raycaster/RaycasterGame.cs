using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Raycaster;

public class RaycasterGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Maze _maze;
    private Player _player;
    private Projector _projector;

    public RaycasterGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _maze = new Maze();
        _player = new Player
        {
            Position = new Point2(Maze.WindowWidth / 2f, Maze.WindowHeight / 2f),
            TurnDirection = TurnDirection.None,
            WalkDirection = WalkDirection.None,
            RotationAngle = MathHelper.PiOver2
        };
        _projector = new Projector();

        _graphics.PreferredBackBufferWidth = Maze.WindowWidth;
        _graphics.PreferredBackBufferHeight = Maze.WindowHeight;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (keyboardState.IsKeyUp(Keys.W) || keyboardState.IsKeyUp(Keys.S))
        {
            _player.WalkDirection = WalkDirection.None;
        }

        if (keyboardState.IsKeyUp(Keys.A) || keyboardState.IsKeyUp(Keys.D))
        {
            _player.TurnDirection = TurnDirection.None;
        }

        if (keyboardState.IsKeyDown(Keys.W))
        {
            _player.WalkDirection = WalkDirection.Forward;
        }
        else if (keyboardState.IsKeyDown(Keys.S))
        {
            _player.WalkDirection = WalkDirection.Backward;
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            _player.TurnDirection = TurnDirection.Left;
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            _player.TurnDirection = TurnDirection.Right;
        }

        _player.Update(_maze, gameTime);
        _player.CastRays(_maze);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _projector.Draw(_spriteBatch, _player);
        _maze.Draw(_spriteBatch, _player);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
