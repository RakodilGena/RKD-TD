using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Visuals;

namespace MonoGameLibrary;

public class Core : Game
{
    private static Core _instance = null!;

    // The scene that is currently active.
    private static Scene? _activeScene;

    // The next scene to switch to, if there is one.
    private static Scene? _nextScene;

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => _instance;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; } = null!;

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public new static GraphicsDevice GraphicsDevice { get; private set; } = null!;

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; } = null!;

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public new static ContentManager Content { get; private set; } = null!;

    /// <summary>
    /// Gets a reference to the input management system.
    /// </summary>
    public static InputManager Input { get; private set; } = null!;

    /// <summary>
    /// Gets or Sets a value that indicates if the game should exit when the esc key on the keyboard is pressed.
    /// </summary>
    public static bool ExitOnEscape { get; set; }

    public new static bool IsMouseVisible
    {
        get => ((Game)_instance).IsMouseVisible;
        set => ((Game)_instance).IsMouseVisible = value;
    }

    public static bool KeepMouseOnScreen { get; set; }

    public static Rectangle VirtualScreenBounds { get; private set; }

    public static ResolutionManager Resolution { get; private set; } = null!;

    public new static bool IsActive => ((Game)_instance).IsActive;

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="targetFps">Target FPS.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    /// <param name="exitOnEscape">Should the game exit when Escape is pressed.</param>
    /// <param name="keepMouseOnScreen">Should the game keep the mouse cursor on screen.</param>
    protected Core(
        string title,
        int width,
        int height,
        int targetFps,
        bool fullScreen,
        bool exitOnEscape,
        bool keepMouseOnScreen)
    {
        // Ensure that multiple cores are not created.
        if (_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        // Store reference to engine for global member access.
        _instance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;
        Graphics.HardwareModeSwitch = !fullScreen;

        // Apply the graphic presentation changes.
        Graphics.ApplyChanges();

        Window.IsBorderless = fullScreen;

        // Set the window title.
        Window.Title = title;

        // Set the core's content manager to a reference of the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Mouse is visible by default.
        IsMouseVisible = true;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / targetFps);

        ExitOnEscape = exitOnEscape;
        KeepMouseOnScreen = keepMouseOnScreen;

        VirtualScreenBounds = new Rectangle(
            0,
            0,
            width,
            height);
    }

    protected override void Initialize()
    {
        // Set the core's graphics device to a reference of the base Game's
        // graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // Create a new input manager.
        Input = new InputManager();

        Resolution = new ResolutionManager(Graphics, GraphicsDevice);
        Resolution.Initialize();

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        // Update the input manager.
        Input.Update(gameTime);

        //implement mouse keep-on-screen here
        if (IsActive && KeepMouseOnScreen)
        {
            DoKeepMouseOnScreen();
        }

        if (ExitOnEscape && Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Exit();
        }

        // if there is a next scene waiting to be switch to,
        // then transition to that scene.
        if (_nextScene is not null)
        {
            TransitionScene();
        }

        // If there is an active scene, update it.
        _activeScene?.Update(gameTime);

        base.Update(gameTime);
    }

    private static void DoKeepMouseOnScreen()
    {
        var mouse = Input.Mouse;
        var virtualMouse = Resolution.ToVirtualMouse(mouse.Position);

        var bounds = VirtualScreenBounds;
        if (bounds.Contains(virtualMouse))
        {
            return;
        }

        int newX = Math.Clamp(virtualMouse.X, bounds.X, bounds.Width - 1);
        int newY = Math.Clamp(virtualMouse.Y, bounds.Y, bounds.Height - 1);

        var newPosition = new Point(newX, newY);
        var screenMousePosition = Resolution.ToScreenMouse(newPosition);

        mouse.SetPosition(screenMousePosition);
    }

    private static void TransitionScene()
    {
        // If there is an active scene, dispose of it.
        _activeScene?.Dispose();

        // Force the garbage collector to collect to ensure memory is cleared.
        GC.Collect();

        // Change the currently active scene to the new scene.
        _activeScene = _nextScene;

        // Null out the next scene value so it does not trigger a change over and over.
        _nextScene = null;

        // If the active scene now is not null, initialize it.
        // Remember, just like with Game, the Initialize call also calls the
        // Scene.LoadContent
        _activeScene?.Initialize();
    }

    public static void ChangeScene(Scene next)
    {
        // Only set the next scene value if it is not the same
        // instance as the currently active scene.
        if (_activeScene != next)
        {
            _nextScene = next;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        // If there is an active scene, draw it.
        _activeScene?.Draw(gameTime);

        base.Draw(gameTime);
    }

    /// <summary>
    /// Exit the game at the end of this tick.
    /// </summary>
    public new static void Exit()
    {
        ((Game)Instance).Exit();
    }
}