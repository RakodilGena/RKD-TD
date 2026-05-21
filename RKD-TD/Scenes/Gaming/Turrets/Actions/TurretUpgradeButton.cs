using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.Turrets.Actions;

internal sealed class TurretUpgradeButton : Button
{
    private const int LABEL_BORDER_WIDTH = 2;

    private readonly Label _textLabel;
    private readonly Label _priceLabel;

    private int _upgradeCost;

    private bool CanUpgrade => _upgradeCost > 0;

    public TurretUpgradeButton(
        Vector2 position,
        Vector2 origin,
        Sprite sprite,
        Color idleColor,
        Color hoveredColor,
        Vector2 scale)
        : base(position, origin, sprite, idleColor, hoveredColor, scale, layerDepth: 1f)
    {
        _textLabel = new BorderedLabel(
            font: GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_ACTION_BUTTON_TEXT))
        {
            Color = Colors.Buttons.Text,
            BorderColor = Colors.Buttons.TextBorders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH)
        };

        _priceLabel = new BorderedLabel(
            font: GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_ACTION_BUTTON_PRICE_TEXT))
        {
            Color = Colors.Game.TurretPrices.Affordable,
            BorderColor = Colors.Game.TurretPrices.Borders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH)
        };
    }

    public void SetUpgradeCost(int cost)
    {
        _upgradeCost = cost;

        var center = Bounds.Center.ToVector2();

        if (CanUpgrade)
        {
            _textLabel.Text = "UPGRADE ";
            _priceLabel.Text = _upgradeCost.ToString();

            var textLabelSize = _textLabel.MeasureText();
            var priceLabelSize = _priceLabel.MeasureText();

            var labelsWidth = textLabelSize.X + priceLabelSize.X;
            var labelHeight = textLabelSize.Y;

            _textLabel.Position = center - new Vector2(labelsWidth, labelHeight) / 2;
            _priceLabel.Position = _textLabel.Position + new Vector2(textLabelSize.X, -1);
        }
        else
        {
            _textLabel.Text = "MAX LVL";
            _priceLabel.Text = "";

            _textLabel.Position = center - _textLabel.MeasureText() / 2;
        }
    }

    public void Update(int userCoins)
    {
        _priceLabel.Color = userCoins >= _upgradeCost
            ? Colors.Game.TurretPrices.Affordable
            : Colors.Game.TurretPrices.Unaffordable;

        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        _textLabel.Draw(spriteBatch);
        _priceLabel.Draw(spriteBatch);
    }
}