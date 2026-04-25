using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.Interfaces;

namespace RKD_TD.Scenes.Gaming;

internal sealed class UserResources : IMyDrawable
{
    private Vector2 _healthSpritePosition, _coinsSpritePosition;

    private readonly Sprite _healthSprite, _coinsSprite;
    private readonly Label _healthLabel, _coinsLabel;

    public int Health { get; }

    public int Coins { get; }

    public float LayerDepth
    {
        get;
        set
        {
            field = value;
            _healthSprite.LayerDepth = value;
            _coinsSprite.LayerDepth = value;
            _healthLabel.LayerDepth = value;
            _coinsLabel.LayerDepth = value;
        }
    }

    public UserResources(
        int health,
        int coins,
        Vector2 position,
        Sprite healthSprite,
        Sprite coinsSprite,
        SpriteFont font)
    {
        Health = health;
        Coins = coins;

        _healthSprite = healthSprite;
        _coinsSprite = coinsSprite;

        _healthLabel = new Label(health.ToString(), font);
        _coinsLabel = new Label(coins.ToString(), font);

        PlaceElements(position);
    }

    private void PlaceElements(Vector2 position)
    {
        const int
            sizePx = 60,
            labelMarginPx = 10,
            resMarginPx = 250;

        var toLabel = new Vector2(sizePx + labelMarginPx, 0);

        _healthSpritePosition = position;

        var healthScaleX = sizePx / _healthSprite.Width;
        var healthScaleY = sizePx / _healthSprite.Height;
        _healthSprite.Scale = new Vector2(healthScaleX, healthScaleY);
        _healthLabel.Position = _healthSpritePosition + toLabel;


        _coinsSpritePosition = position + new Vector2(resMarginPx, 0);
        var coinsScaleX = sizePx / _coinsSprite.Width;
        var coinsScaleY = sizePx / _coinsSprite.Height;
        _coinsSprite.Scale = new Vector2(coinsScaleX, coinsScaleY);
        _coinsLabel.Position = _coinsSpritePosition + toLabel;
    }

    public static UserResources FromFile(
        XDocument map,
        TextureAtlas gameObjectsTextures,
        Vector2 position)
    {
        XElement root = map.Root!;

        var healthElement = root.Element("Health")!;
        var health = int.Parse(healthElement.Value);
        var healthTexture = healthElement.Attribute("textureAlias")!.Value;
        var healthSprite = gameObjectsTextures.CreateSprite(healthTexture);

        var coinsElement = root.Element("Coins")!;
        var coins = int.Parse(coinsElement.Value);
        var coinsTexture = coinsElement.Attribute("textureAlias")!.Value;
        var coinsSprite = gameObjectsTextures.CreateSprite(coinsTexture);

        var font = GlobalAssets.FontAtlas.GetFont(Fonts.USER_RESOURCES);

        return new UserResources(
            health,
            coins,
            position,
            healthSprite,
            coinsSprite,
            font);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _healthSprite.Draw(
            spriteBatch,
            _healthSpritePosition);

        _healthLabel.Draw(
            spriteBatch);


        _coinsSprite.Draw(
            spriteBatch,
            _coinsSpritePosition);

        _coinsLabel.Draw(
            spriteBatch);
    }
}