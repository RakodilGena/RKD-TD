using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Graphics.Tiles;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.Gaming.Enemies.Spawns;
using RKD_TD.Scenes.Gaming.Explosions;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Misc;
using RKD_TD.Scenes.Gaming.PauseMenus;
using RKD_TD.Scenes.Gaming.Projectiles;
using RKD_TD.Scenes.Gaming.Turrets.Active;
using RKD_TD.Scenes.Gaming.Turrets.Purchase;
using RKD_TD.Scenes.MapSelection;

namespace RKD_TD.Scenes.Gaming;

internal sealed class GamingScene : Scene
{
    private readonly string _mapFile;

    private const string
        BACKGROUND_TEXTURE_PATH = "images/game/background",
        GAME_OBJECTS_ATLAS_NAME = "images/game/game-objects-atlas-definition.xml",
        ENEMY_CONFIG_NAME = "configs/enemyconfig.xml",
        HEALTH_BAR_CONFIG_NAME = "configs/healthbarconfig.xml",
        TURRET_CONFIG_NAME = "configs/turretconfig.xml";

    private PauseMenu _pauseMenu = null!;

    private Sprite _backgroundSprite = null!;
    private TextureAtlas _gameObjectsTextures = null!;

    private GameState _gameState = GameState.Normal;
    private PendingTurret? _pendingTurret;

    private Camera _camera = null!;
    private GameClockWidget _gameClockWidget = null!;
    private FpsMeter _fpsMeter = null!;

    private Tilemap _map = null!;
    private Portals _portals = null!;
    private UserResources _userResources = null!;

    private TurretPurchasePanel _turretPurchasePanel = null!;
    private PendingTurretStash _pendingTurretStash = null!;
    private BuildGrid _buildGrid = null!;
    private BuildCell? _hoveredCell;
    private TurretFactory _turretFactory = null!;

    private EnemySpawner _enemySpawner = null!;
    private int _pendingWaveReward;
    private bool _allWavesSpawned;

    private readonly HashSet<Enemy> _enemies = [];
    private readonly HashSet<Turret> _turrets = [];
    private readonly HashSet<Projectile> _projectiles = [];
    private readonly HashSet<Flash> _flashes = [];
    private readonly HashSet<Explosion> _explosions = [];

    private event EventHandler<Enemy>? EnemyRemoved;

    public GamingScene(string mapFile)
    {
        _mapFile = mapFile;
    }

    public override void Initialize()
    {
        base.Initialize();

        //after base.Initialize() all graphic elements must have been created

        _fpsMeter = new FpsMeter(new Vector2(1600, 32));
        InitCamera();
        InitGameClockWidget();
        InitTurretPurchasePanel();
        InitPauseMenu();

        //free the memory after the atlas no longer needed.
        _gameObjectsTextures = null!;
    }

    private void InitCamera()
    {
        _camera = new Camera(
            maxZoom: 1,
            zoomSpeed: 2f,
            cameraMoveSpeed: 400,
            _map,
            putToCenter: true,
            mapBordersMargin: 150,
            extraBottomMargin: 130);

        _map.Camera = _camera;
        _portals.Camera = _camera;
    }

    private void InitGameClockWidget()
    {
        _gameClockWidget = new GameClockWidget(
            new Vector2(1840, 40),
            _gameObjectsTextures)
        {
            LayerDepth = 0.9f
        };
    }

    private void InitTurretPurchasePanel()
    {
        _turretPurchasePanel = new TurretPurchasePanel(
            position: new Vector2(10, 915),
            _gameObjectsTextures,
            _pendingTurretStash,
            scale: 0.6f,
            panelLayerDepth: 0.9f,
            buttonLayerDepth: 0.91f);
        _turretPurchasePanel.TurretPicked += BeginTurretPlacing;
    }

    private void InitPauseMenu()
    {
        _pauseMenu = new PauseMenu(
            topY: 200,
            _gameObjectsTextures);

        _pauseMenu.Resumed += (_, _) => ResumeGame();
        _pauseMenu.ExitedToMainMenu += (_, _) => ToMapSelection();
        _pauseMenu.ExitedGame += (_, _) => Core.Exit();
    }

    public override void LoadContent()
    {
        base.LoadContent();

        LoadBackground();

        var mapDoc = XmlLoader.Load(Content, _mapFile);

        LoadGameObjectsTextures();

        LoadMap(mapDoc);

        LoadPortals(mapDoc);

        LoadUserResources(mapDoc);

        LoadEnemySpawner(mapDoc);

        LoadBuildGrid(mapDoc);

        LoadTurretFactory();
    }

    private void LoadBackground()
    {
        var texture = Content.Load<Texture2D>(BACKGROUND_TEXTURE_PATH);
        var textureRegion = new TextureRegion(texture, 0, 0, 1920, 1080);

        _backgroundSprite = new Sprite(textureRegion);
    }

    private void LoadGameObjectsTextures()
    {
        _gameObjectsTextures = TextureAtlas.FromFile(
            Content,
            GAME_OBJECTS_ATLAS_NAME);
    }

    private void LoadMap(XDocument mapDoc)
    {
        _map = Tilemap.FromFile(Content, mapDoc);
        _map.Scale = Vector2.One;
        _map.LayerDepth = 0;
    }

    private void LoadPortals(XDocument mapDoc)
    {
        _portals = Portals.FromFile(mapDoc, _gameObjectsTextures);
        _portals.LayerDepth = 0.1f;
        _portals.Scale = new Vector2(1.2f);
    }

    private void LoadUserResources(XDocument mapDoc)
    {
        _userResources = UserResources.FromFile(
            mapDoc,
            _gameObjectsTextures,
            new Vector2(30));
        _userResources.LayerDepth = 0.9f;
        _userResources.CriticalDamageReceived += OnCriticalDamageReceived;
    }

    private void LoadEnemySpawner(XDocument mapDoc)
    {
        var enemyConfigDoc = XmlLoader.Load(Content, ENEMY_CONFIG_NAME);
        var healthBarConfigDoc = XmlLoader.Load(Content, HEALTH_BAR_CONFIG_NAME);

        _enemySpawner = EnemySpawner.FromFile(
            mapDoc,
            enemyConfigDoc,
            healthBarConfigDoc,
            _gameObjectsTextures,
            new Vector2(600, 32));

        _enemySpawner.EnemySpawned += OnEnemySpawned;
        _enemySpawner.AllWavesFinished += OnAllWavesFinished;
        _enemySpawner.WaveFinished += OnWaveFinished;

        _enemySpawner.Resume();
    }

    private void LoadBuildGrid(XDocument mapDoc)
    {
        _buildGrid = BuildGrid.FromMap(mapDoc);
    }

    private void LoadTurretFactory()
    {
        var turretCfg = XmlLoader.Load(Content, TURRET_CONFIG_NAME);
        _turretFactory = TurretFactory.FromFile(
            turretCfg,
            _gameObjectsTextures,
            out _pendingTurretStash);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(30, 30, 30));

        //order of calling draw is important here because apparently by default layerDepth is ignored
        //thus the later draw is called, the higher it is drawn.
        //decided to keep it as is for the time being.
        var sb = Core.SpriteBatch;
        sb.Begin();

        _backgroundSprite.Draw(sb, Vector2.Zero);

        _map.Draw(sb);
        _portals.Draw(sb);

        foreach (var enemy in _enemies)
        {
            enemy.Draw(sb);
        }

        foreach (var turret in _turrets)
        {
            turret.Draw(sb);
        }

        foreach (var projectile in _projectiles)
        {
            projectile.Draw(sb);
        }

        foreach (var flash in _flashes)
        {
            flash.Draw(sb);
        }

        foreach (var explosion in _explosions)
        {
            explosion.Draw(sb);
        }

        _userResources.Draw(sb);
        _enemySpawner.Draw(sb);
        //_fpsMeter.Draw(sb);
        _gameClockWidget.Draw(sb);

        if (_gameState is GameState.PlacingTurret)
        {
            _hoveredCell?.DrawPlacementOverlay(
                sb,
                _pendingTurret,
                _camera);
        }
        else
        {
            _turretPurchasePanel.Draw(sb);

            if (_gameState is GameState.InPauseMenu)
            {
                _pauseMenu.Draw(sb);
            }
        }

        sb.End();
        base.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        HandleInput();

        if (_gameState is GameState.InPauseMenu)
        {
            _pauseMenu.Update();
            return;
        }

        var clockDelta = _gameClockWidget.GetDelta(gameTime);
        var uiDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _camera.Update(uiDelta);
        _fpsMeter.Update(uiDelta);

        if (_gameState is GameState.PlacingTurret)
        {
            UpdatePlacementMode();
        }
        else
        {
            _turretPurchasePanel.Update();
            _gameClockWidget.Update();
        }

        _portals.Update(clockDelta);

        foreach (var enemy in _enemies)
        {
            enemy.Update(clockDelta);
        }

        foreach (var flash in _flashes)
        {
            flash.Update(clockDelta);
        }

        foreach (var explosion in _explosions)
        {
            explosion.Update(clockDelta, _enemies);
        }

        foreach (var projectile in _projectiles)
        {
            projectile.Update(_enemies, clockDelta);
        }

        foreach (var turret in _turrets)
        {
            turret.Update(clockDelta, _enemies);
        }

        _enemySpawner.Update(clockDelta);


        base.Update(gameTime);
    }

    private void HandleInput()
    {
        var kb = Core.Input.Keyboard;
        var mouse = Core.Input.Mouse;

        if (_gameState is GameState.InPauseMenu)
        {
            if (kb.WasKeyJustPressed(Keys.Escape))
            {
                _gameState = GameState.Normal;
                return;
            }
        }

        if (_gameState is GameState.PlacingTurret)
        {
            if (kb.WasKeyJustPressed(Keys.Escape) || mouse.WasButtonJustPressed(MouseButton.Right))
            {
                CancelTurretPlacing();
            }
            else if (mouse.WasButtonJustPressed(MouseButton.Left))
            {
                var mousePosition = mouse.Position;

                var cell = _buildGrid.GetCellAtWorld(
                    mousePosition.ToVector2(),
                    _camera);

                if (cell is { IsBuildable: true, IsOccupied: false })
                {
                    if (PlaceTurret(cell))
                        CancelTurretPlacing();
                }
            }
        }
        else if (_gameState is GameState.Normal)
        {
            if (kb.WasKeyJustPressed(Keys.Escape))
            {
                _gameState = GameState.InPauseMenu;
                return;
            }
        }

        if (kb.WasKeyJustPressed(Keys.Space))
        {
            _gameClockWidget.SwitchPaused();
        }
        else if (kb.WasKeyJustPressed(Keys.D1))
        {
            _gameClockWidget.SetSpeed(speedIndex: 1);
        }
        else if (kb.WasKeyJustPressed(Keys.D2))
        {
            _gameClockWidget.SetSpeed(speedIndex: 2);
        }
        else if (kb.WasKeyJustPressed(Keys.D3))
        {
            _gameClockWidget.SetSpeed(speedIndex: 3);
        }
    }

    private void OnEnemySpawned(object? sender, Enemy[] enemies)
    {
        foreach (var enemy in enemies)
        {
            enemy.Camera = _camera;

            enemy.ReachedPortal += OnEnemyReachedPortal;
            enemy.Destroyed += OnEnemyDestroyed;

            _enemies.Add(enemy);
        }
    }

    private void OnEnemyReachedPortal(object? sender, int damage)
    {
        _userResources.ReceiveDamage(damage);
        RemoveEnemy((Enemy)sender!);
    }

    private void OnEnemyDestroyed(object? sender, int reward)
    {
        _userResources.GainCoins(reward);
        RemoveEnemy((Enemy)sender!);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        EnemyRemoved?.Invoke(this, enemy);

        if (_enemies.Count is 0)
        {
            if (_pendingWaveReward > 0)
            {
                //all enemies are killed, gain wave reward, resume spawner
                _userResources.GainCoins(_pendingWaveReward);
                _pendingWaveReward = 0;
                _enemySpawner.Resume();
                return;
            }

            if (_allWavesSpawned && _userResources.Health > 0)
            {
                Console.WriteLine("All enemies are successfully destroyed, GAME WON");
                ToMapSelection();
            }
        }
    }

    private void BeginTurretPlacing(object? sender, TurretType turretType)
    {
        var pendingTurret = _pendingTurretStash.GetPendingTurret(turretType);

        if (pendingTurret.Price > _userResources.Coins)
            return;

        _pendingTurret = pendingTurret;
        _gameState = GameState.PlacingTurret;
        Core.IsMouseVisible = false;
    }

    private void UpdatePlacementMode()
    {
        var mousePosition = Core.Input.Mouse.Position.ToVector2();
        _hoveredCell = _buildGrid.GetCellAtWorld(mousePosition, _camera);
    }

    private bool PlaceTurret(BuildCell cell)
    {
        if (_pendingTurret is null)
            return false;

        Console.WriteLine($"TURRET {_pendingTurret.Type} placed at [{cell.WorldPosition.X},{cell.WorldPosition.Y}]");

        var turret = _turretFactory.CreateTurret(cell, _pendingTurret.Type);

        turret.Camera = _camera;
        EnemyRemoved += turret.OnEnemyRemoved;
        turret.ProjectilesFired += OnProjectilesFired;
        _turrets.Add(turret);

        _userResources.GainCoins(-_pendingTurret.Price);
        return true;
    }

    private void CancelTurretPlacing()
    {
        _gameState = GameState.Normal;
        _pendingTurret = null;
        Core.IsMouseVisible = true;
    }

    private void RemoveTurret(Turret turret)
    {
        _turrets.Remove(turret);

        turret.OccupiedCell.IsOccupied = false;
        EnemyRemoved -= turret.OnEnemyRemoved;
    }

    private void OnProjectilesFired(object? sender, TurretShotEventArgs e)
    {
        foreach (var projectile in e.Projectiles)
        {
            projectile.Exhausted += OnProjectileExhausted;
            projectile.Collided += OnProjectileCollided;
            projectile.TrailFlashEmitted += (_, flash) => AddFlash(flash);
            projectile.Camera = _camera;
            _projectiles.Add(projectile);
        }

        foreach (var flash in e.Flashes)
        {
            AddFlash(flash);
        }
    }

    private void AddFlash(Flash flash)
    {
        flash.Finished += OnFlashFinished;
        flash.Camera = _camera;
        _flashes.Add(flash);
    }

    private void OnFlashFinished(object? sender, EventArgs e)
    {
        _flashes.Remove((Flash)sender!);
    }

    private void OnProjectileExhausted(object? sender, EventArgs e)
    {
        RemoveProjectile((Projectile)sender!);
    }

    private void OnProjectileCollided(object? sender, Explosion explosion)
    {
        RemoveProjectile((Projectile)sender!);

        explosion.Finished += OnExplosionFinished;
        explosion.Camera = _camera;
        _explosions.Add(explosion);
    }

    private void OnExplosionFinished(object? sender, EventArgs e)
    {
        _explosions.Remove((Explosion)sender!);
    }

    private void RemoveProjectile(Projectile projectile)
    {
        _projectiles.Remove(projectile);
    }


    private void OnCriticalDamageReceived(object? sender, EventArgs e)
    {
        Console.WriteLine("Critical damage");
        ToMapSelection();
    }

    private void OnWaveFinished(object? sender, int reward)
    {
        _pendingWaveReward = reward;
    }

    private void OnAllWavesFinished(object? sender, EventArgs e)
    {
        _allWavesSpawned = true;
    }

    private void ResumeGame()
    {
        _gameState = GameState.Normal;
    }

    private static void ToMapSelection()
    {
        Core.ChangeScene(new MapSelectionScene());
    }
}