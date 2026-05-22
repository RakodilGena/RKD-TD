using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
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
        width: GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, //1280
        height: GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, //720
        targetFps: 60,
        fullScreen: true,
        exitOnEscape: false)
    {
        IsMouseVisible = false;
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

        Cursor = new GameCursor(idle, pressed);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        Cursor.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}