using Microsoft.Xna.Framework;
using MonoGameLibrary;
using RKD_TD.Assets;
using RKD_TD.Scenes;

namespace RKD_TD;

internal sealed class GameCore : Core
{
    public GameCore() : base(
        title: "RKD Tower Defense",
        width: 1280, //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
        height: 720, //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
        targetFps: 60,
        fullScreen: false,
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