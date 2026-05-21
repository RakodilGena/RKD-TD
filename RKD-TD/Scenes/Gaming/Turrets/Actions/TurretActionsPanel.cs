using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Turrets.Actions;

internal sealed class TurretActionsPanel
{
    private const int
        SIZE_X = 300,
        SIZE_Y = 245,
        LABEL_OFFSET_Y = 20,
        LABEL_BORDER_WIDTH = 3,
        UPGRADE_BUTTON_OFFSET_Y = 85,
        SELL_BUTTON_OFFSET_Y = 155,
        BUTTON_SIZE_X = 260,
        BUTTON_SIZE_Y = 60;

    private static readonly Color ButtonIdleColor = Color.White;
    private static readonly Color ButtonHoveredColor = Color.DarkGray;

    public event EventHandler? UpgradeButtonClicked;
    public event EventHandler? SellButtonClicked;

    private readonly Vector2 _position;
    private readonly Sprite _panelSprite;
    private readonly Label _turretDefinition, _turretLevel;

    public Rectangle Bounds { get; }

    private readonly TurretUpgradeButton _upgradeButton;
    private readonly TurretSellButton _sellButton;

    public TurretActionsPanel(
        Vector2 position,
        TextureAtlas textures)
    {
        _position = position;

        _panelSprite = textures.CreateSprite(Textures.Game.PANEL_400_300);
        _panelSprite.Scale = new Vector2(SIZE_X / _panelSprite.Width, SIZE_Y / _panelSprite.Height);


        _turretDefinition = new BorderedLabel(
            font: GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_ACTION_PANEL_LABEL_TEXT))
        {
            Color = Colors.Game.TurretLabels.Text,
            BorderColor = Colors.Game.TurretLabels.Borders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH)
        };

        _turretLevel = new BorderedLabel(
            font: GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_ACTION_PANEL_LVL_TEXT))
        {
            Color = Colors.Game.TurretLabels.Text,
            BorderColor = Colors.Game.TurretLabels.Borders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH)
        };

        Bounds = new Rectangle(
            (int)_position.X,
            (int)_position.Y,
            SIZE_X,
            SIZE_Y);


        var texture = textures.GetRegion(Textures.Game.BUTTON);
        Vector2 buttonSize = new Vector2(BUTTON_SIZE_X, BUTTON_SIZE_Y);
        var buttonScale = buttonSize / new Vector2(texture.Width, texture.Height);

        var center = Bounds.Center;

        var upgradeButtonSprite = new Sprite(texture);

        var upgradeButtonPosition = new Vector2(
            center.X - buttonSize.X / 2,
            Bounds.Y + UPGRADE_BUTTON_OFFSET_Y);

        _upgradeButton = new TurretUpgradeButton(
            upgradeButtonPosition,
            origin: Vector2.Zero,
            upgradeButtonSprite,
            ButtonIdleColor,
            ButtonHoveredColor,
            buttonScale);
        _upgradeButton.Clicked += (_, _) => UpgradeButtonClicked?.Invoke(this, EventArgs.Empty);


        var sellButtonPosition = new Vector2(
            center.X - buttonSize.X / 2,
            Bounds.Y + SELL_BUTTON_OFFSET_Y);

        var sellButtonSprite = new Sprite(texture);

        _sellButton = new TurretSellButton(
            sellButtonPosition,
            origin: Vector2.Zero,
            sellButtonSprite,
            ButtonIdleColor,
            ButtonHoveredColor,
            buttonScale);
        _sellButton.Clicked += (_, _) => SellButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    public void Initialize(Turret selectedTurret)
    {
        _turretDefinition.Text = $"{selectedTurret.Name} LVL ";
        _turretLevel.Text = $"{selectedTurret.GetLevel() + 1}";

        var panelMiddleX = _panelSprite.Width / 2;

        var turretDefinitionSizeX = _turretDefinition.MeasureText().X;

        var labelsLenght = turretDefinitionSizeX + _turretLevel.MeasureText().X;

        _turretDefinition.Position = _position + new Vector2(panelMiddleX - labelsLenght / 2, LABEL_OFFSET_Y);
        _turretLevel.Position = _turretDefinition.Position + new Vector2(turretDefinitionSizeX, -1);

        _upgradeButton.SetUpgradeCost(selectedTurret.GetUpgradePrice());
        _sellButton.SetSellCost(selectedTurret.GetSellPrice());
    }

    public void Update(int userCoins)
    {
        _upgradeButton.Update(userCoins);
        _sellButton.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _panelSprite.Draw(spriteBatch, _position);
        _turretDefinition.Draw(spriteBatch);
        _turretLevel.Draw(spriteBatch);

        _upgradeButton.Draw(spriteBatch);
        _sellButton.Draw(spriteBatch);
    }
}