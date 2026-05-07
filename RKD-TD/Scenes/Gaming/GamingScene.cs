using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.Gaming.Misc;
using RKD_TD.Scenes.Gaming.PurchaseTurrets;
using RKD_TD.Scenes.MapSelection;

namespace RKD_TD.Scenes.Gaming;

internal sealed class GamingScene : Scene
{
    private readonly string _mapFile;
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
    private BuildGrid _buildGrid = null!;
    private BuildCell? _hoveredCell;

    private EnemySpawner _enemySpawner = null!;
    private int _pendingWaveReward;
    private bool _allWavesSpawned;

    private readonly HashSet<Enemy> _enemies = [];

    public GamingScene(string mapFile)
    {
        _mapFile = mapFile;
    }

    public override void Initialize()
    {
        base.Initialize();

        //after base.Initialize() all graphic elements must be created

        _fpsMeter = new FpsMeter(new Vector2(1600, 30));
        InitCamera();
        InitGameClockWidget();
        InitTurretPurchasePanel();
    }

    private void InitCamera()
    {
        _camera = new Camera(
            initialZoom: 1,
            maxZoom: 2,
            zoomSpeed: 2f,
            cameraMoveSpeed: 400,
            _map,
            putToCenter: true,
            mapBordersMargin: 150);

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
            scale: 0.6f,
            panelLayerDepth: 0.9f,
            buttonLayerDepth: 0.91f);
        _turretPurchasePanel.TurretPicked += BeginTurretPlacing;

        PendingTurretStash.InitPendingTurrets(_camera);
    }

    public override void LoadContent()
    {
        base.LoadContent();

        var mapDoc = XmlLoader.Load(Content, _mapFile);

        LoadGameObjectsTextures(mapDoc);

        LoadMap(mapDoc);

        LoadPortals(mapDoc);

        LoadUserResources(mapDoc);

        LoadEnemySpawner(mapDoc);

        LoadBuildGrid(mapDoc);
    }

    private void LoadGameObjectsTextures(XDocument mapDoc)
    {
        string atlasName = mapDoc.Root!.Element("GameObjectsAtlas")!.Value;
        _gameObjectsTextures = TextureAtlas.FromFile(
            Content,
            atlasName);
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
        _enemySpawner = EnemySpawner.FromFile(
            mapDoc,
            _gameObjectsTextures,
            new Vector2(600, 30));

        _enemySpawner.EnemySpawned += OnEnemySpawned;
        _enemySpawner.AllWavesFinished += OnAllWavesFinished;
        _enemySpawner.WaveFinished += OnWaveFinished;

        _enemySpawner.Resume();
    }

    private void LoadBuildGrid(XDocument mapDoc)
    {
        _buildGrid = BuildGrid.FromMap(mapDoc);
    }


    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.PeachPuff);

        //order of calling draw is important here because apparently by default layerDepth is ignored
        //thus the later draw is called, the higher it is drawn.
        //decided to keep it as is for the time being.
        var sb = Core.SpriteBatch;
        sb.Begin();

        _map.Draw(sb);
        _portals.Draw(sb);

        foreach (var enemy in _enemies)
        {
            enemy.Draw(sb);
        }

        _userResources.Draw(sb);
        _enemySpawner.Draw(sb);
        _fpsMeter.Draw(sb);
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
        }


        sb.End();
        base.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        HandleInput();

        var clockDelta = _gameClockWidget.GetDelta(gameTime);
        var uiDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _camera.Update(uiDelta);
        _fpsMeter.Update(uiDelta);

        if (_gameState is GameState.PlacingTurret)
        {
            var mousePosition = Core.Input.Mouse.Position.ToVector2();
            UpdatePlacementMode(mousePosition);
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

        _enemySpawner.Update(clockDelta);


        base.Update(gameTime);
    }

    private void HandleInput()
    {
        var kb = Core.Input.Keyboard;
        var mouse = Core.Input.Mouse;
        if (_gameState is GameState.Normal)
        {
            if (kb.WasKeyJustPressed(Keys.Escape))
            {
                Core.ChangeScene(new MapSelectionScene());
                return;
            }
        }
        else
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
                    PlaceTurret(cell);
                    CancelTurretPlacing();
                }
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
                Core.ChangeScene(new MapSelectionScene());
            }
        }
    }

    private void BeginTurretPlacing(object? sender, PendingTurretType turretType)
    {
        var pendingTurret = PendingTurretStash.GetPendingTurret(turretType);

        if (pendingTurret.Price > _userResources.Coins)
            return;

        _pendingTurret = pendingTurret;
        _gameState = GameState.PlacingTurret;
        Core.IsMouseVisible = false;
    }

    private void CancelTurretPlacing()
    {
        _gameState = GameState.Normal;
        _pendingTurret = null;
        Core.IsMouseVisible = true;
    }

    private void UpdatePlacementMode(Vector2 mouseWorld)
    {
        _hoveredCell = _buildGrid.GetCellAtWorld(mouseWorld, _camera);
    }

    private void PlaceTurret(BuildCell cell)
    {
        if (_pendingTurret is null)
            return;

        var mouse = Core.Input.Mouse.Position;
        Console.WriteLine($"TURRET {_pendingTurret.Type} placed at [{cell.WorldPosition.X},{cell.WorldPosition.Y}]");
        Console.WriteLine($"MOUSE: {mouse.X},{mouse.Y}");

        cell.IsOccupied = true;
    }

    private void OnCriticalDamageReceived(object? sender, EventArgs e)
    {
        Console.WriteLine("Critical damage");
        Core.ChangeScene(new MapSelectionScene());
    }

    private void OnWaveFinished(object? sender, int reward)
    {
        _pendingWaveReward = reward;
    }

    private void OnAllWavesFinished(object? sender, EventArgs e)
    {
        _allWavesSpawned = true;
    }
}