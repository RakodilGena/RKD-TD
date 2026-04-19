using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Title;

internal sealed class TitleScene : Scene
{
    private Label _gameTitle = null!;
    private TitleMenu _titleMenu = null!;

    public override void Initialize()
    {
        base.Initialize();

        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        var kwFont180 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_TITLE);

        const string mainTitleText = "RKD TOWER DEFENSE";
        var textCenter = kwFont180.MeasureString(mainTitleText) / 2;

        _gameTitle = new Label(
            position: new Vector2(
                screenCenter,
                80),
            origin: new Vector2(
                textCenter.X,
                0),
            mainTitleText,
            kwFont180,
            Color.Black,
            scale: 1,
            layerDepth: 1);

        _titleMenu = new TitleMenu(new Vector2(65, 435));
    }


    public override void Update(GameTime gameTime)
    {
        _titleMenu.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;

        sb.Begin();

        _gameTitle.Draw(sb);
        _titleMenu.Draw(sb);

        sb.End();

        base.Draw(gameTime);
    }
}