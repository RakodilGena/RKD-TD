using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.MapSelection;

namespace RKD_TD.Scenes.Gaming;

internal sealed class GamingScene : Scene
{
    //titles and meters

    private readonly string _mapFile;
    private TextureAtlas _gameObjectsTextures = null!;


    private Camera _camera = null!;
    private GameClock _gameClock = null!;
    private FpsMeter _fpsMeter = null!;


    private Tilemap _map = null!;
    private Portals _portals = null!;
    private UserResources _userResources = null!;
    private EnemySpawner _enemySpawner = null!;
    private HashSet<Enemy> _enemies = [];


    public GamingScene(string mapFile)
    {
        _mapFile = mapFile;
    }

    public override void Initialize()
    {
        base.Initialize();

        //after base.Initialize() all graphic elements must be created

        InitCamera();
        _gameClock = new GameClock();
        _fpsMeter = new FpsMeter(new Vector2(1600, 30));
    }

    private void InitCamera()
    {
        _camera = new Camera(
            initialZoom: 1,
            maxZoom: 2,
            zoomSpeed: 1f,
            cameraMoveSpeed: 400,
            _map,
            putToCenter: true,
            mapBordersMargin: 60);

        _map.Camera = _camera;
        _portals.Camera = _camera;
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

        _enemySpawner.Start();
    }


    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;
        sb.Begin();

        _map.Draw(sb);
        _portals.Draw(sb);
        _userResources.Draw(sb);
        _enemySpawner.Draw(sb);
        _fpsMeter.Draw(sb);

        foreach (var enemy in _enemies)
        {
            enemy.Draw(sb);
        }

        sb.End();
        base.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        HandleInput();

        var clockDelta = _gameClock.GetDelta(gameTime);
        var uiDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        _fpsMeter.Update(uiDelta);
        _camera.Update(uiDelta);

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
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new MapSelectionScene());
            return;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            if (_gameClock.IsPaused)
            {
                _gameClock.Resume();
            }
            else
            {
                _gameClock.Pause();
            }
        }
    }

    private void OnEnemySpawned(object? sender, Enemy enemy)
    {
        enemy.Camera = _camera;
        _enemies.Add(enemy);

        enemy.ReachedPortal += OnEnemyReachedPortal;
        enemy.Destroyed += OnEnemyDestroyed;
    }

    private void OnEnemyReachedPortal(object? sender, int damage)
    {
        RemoveEnemy((Enemy)sender!);
        _userResources.ReceiveDamage(damage);
    }

    private void OnEnemyDestroyed(object? sender, int reward)
    {
        RemoveEnemy((Enemy)sender!);
        _userResources.GainCoins(reward);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }

    private void OnCriticalDamageReceived(object? sender, EventArgs e)
    {
        Console.WriteLine("Critical damage");
        Core.ChangeScene(new MapSelectionScene());
    }

    private void OnWaveFinished(object? sender, int reward)
    {
        _userResources.GainCoins(reward);
    }

    private void OnAllWavesFinished(object? sender, EventArgs e)
    {
        Console.WriteLine("AllWavesFinished");
    }
}