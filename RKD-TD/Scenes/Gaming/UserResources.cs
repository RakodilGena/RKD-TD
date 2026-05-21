using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;

namespace RKD_TD.Scenes.Gaming;

internal sealed class UserResources
{
    private const int
        ICON_SIZE_PX = 40,
        LABEL_MARGIN_X_PX = 10,
        COINS_MARGIN_PX = 60,
        LABEL_PADDING_Y_PX = -4,
        LABEL_BORDER_WIDTH = 2;


    private Vector2 _healthSpritePosition, _coinsSpritePosition;

    private readonly Sprite _healthSprite, _coinsSprite;
    private readonly Label _healthLabel, _coinsLabel;

    public int Health
    {
        get;
        private set
        {
            field = value <= 0 ? 0 : value;

            _healthLabel.Text = field.ToString();
        }
    }

    public int Coins
    {
        get;
        private set
        {
            field = value;
            _coinsLabel.Text = field.ToString();
        }
    }

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
        _healthSprite = healthSprite;
        _coinsSprite = coinsSprite;

        _healthLabel = new BorderedLabel(font)
        {
            Color = Colors.Game.Labels.Text,
            BorderColor = Colors.Game.Labels.TextBorders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH)
        };
        _coinsLabel = new BorderedLabel(font)
        {
            Color = Colors.Game.Labels.Text,
            BorderColor = Colors.Game.Labels.TextBorders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH)
        };

        Health = health;
        Coins = coins;

        PlaceElements(position);
    }

    private void PlaceElements(Vector2 position)
    {
        var toLabel = new Vector2(ICON_SIZE_PX + LABEL_MARGIN_X_PX, LABEL_PADDING_Y_PX);
        _healthSpritePosition = position;
        var healthScaleX = ICON_SIZE_PX / _healthSprite.Width;
        var healthScaleY = ICON_SIZE_PX / _healthSprite.Height;
        _healthSprite.Scale = new Vector2(healthScaleX, healthScaleY);
        _healthLabel.Position = _healthSpritePosition + toLabel;


        _coinsSpritePosition = position + new Vector2(0, COINS_MARGIN_PX);
        var coinsScaleX = ICON_SIZE_PX / _coinsSprite.Width;
        var coinsScaleY = ICON_SIZE_PX / _coinsSprite.Height;
        _coinsSprite.Scale = new Vector2(coinsScaleX, coinsScaleY);
        _coinsLabel.Position = _coinsSpritePosition + toLabel;
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

    public void GainCoins(int amount) => Coins += amount;

    public void ReceiveDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            CriticalDamageReceived?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? CriticalDamageReceived;

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

        var font = GlobalAssets.FontAtlas.GetFont(Fonts.USER_RESOURCES_DIGITS);

        return new UserResources(
            health,
            coins,
            position,
            healthSprite,
            coinsSprite,
            font);
    }
}