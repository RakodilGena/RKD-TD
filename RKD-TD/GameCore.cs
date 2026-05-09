using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using RKD_TD.Assets;
using RKD_TD.Scenes.Title;

namespace RKD_TD;

internal sealed class GameCore : Core
{
    public const bool DRAW_HIT_BOX = false;

    public GameCore() : base(
        title: "RKD Tower Defense",
        width: GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, //1280
        height: GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, //720
        targetFps: 60,
        fullScreen: true,
        exitOnEscape: true)
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
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}