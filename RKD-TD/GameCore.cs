using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Visuals;
using RKD_TD.Assets;
using RKD_TD.Models.UI;
using RKD_TD.Scenes.Title;

namespace RKD_TD;

internal sealed class GameCore : Core
{
    public const bool DRAW_HIT_BOX = false;

    public static GameCursor Cursor = null!;

    public GameCore() : base(
        title: "RKD Tower Defense",
        width: ResolutionManager.VirtualWidth,
        height: ResolutionManager.VirtualHeight,
        targetFps: 120,
        fullScreen: true,
        exitOnEscape: false,
        keepMouseOnScreen: true)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();

        var titleScene = new TitleScene();
        ChangeScene(titleScene);
    }

    protected override void LoadContent()
    {
        GlobalAssets.Load(Content);

        var atlas = TextureAtlas.FromFile(
            Content,
            "images/cursor/cursor-atlas-definition.xml");

        var idle = atlas.GetRegion(Textures.Cursor.IDLE);
        var pressed = atlas.GetRegion(Textures.Cursor.PRESSED);

        IsMouseVisible = false;
        Cursor = new GameCursor(idle, pressed);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        Cursor.Update();
    }
}