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
        SIZE_Y = 325,
        LABEL_OFFSET_Y = 20,
        LABEL_BORDER_WIDTH = 3,
        DAMAGE_LABEL_OFFSET_Y = 95,
        DAMAGE_COUNTER_OFFSET_Y = 130,
        DAMAGE_COUNTER_BORDER_WIDTH = 2,
        UPGRADE_BUTTON_OFFSET_Y = 155,
        SELL_BUTTON_OFFSET_Y = 225,
        BUTTON_SIZE_X = 260,
        BUTTON_SIZE_Y = 60;

    private static readonly float
        DamageLabelScale = 0.7f,
        DamageCounterScaleX = 0.8f;

    private static readonly Color ButtonIdleColor = Color.White;
    private static readonly Color ButtonHoveredColor = Color.DarkGray;

    public event EventHandler? UpgradeButtonClicked;
    public event EventHandler? SellButtonClicked;

    private readonly Vector2 _position;
    private readonly Sprite _panelSprite;

    private readonly Label
        _turretDefinition,
        _turretLevel,
        _damageDealtLabel,
        _damageDealtCounter;

    private int _damageDealt = -1;

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

        var labelTextFont = GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_ACTION_PANEL_LABEL_TEXT);
        var levelTextFont = GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_ACTION_PANEL_LVL_TEXT);

        _turretDefinition = new BorderedLabel(labelTextFont)
        {
            Color = Colors.Game.TurretLabels.Text,
            BorderColor = Colors.Game.TurretLabels.Borders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH),
        };

        _turretLevel = new BorderedLabel(levelTextFont)
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


        _damageDealtLabel = new BorderedLabel(labelTextFont)
        {
            Text = "Damage dealt: ",
            Scale = new Vector2(DamageLabelScale),
            Color = Colors.Game.TurretLabels.Text,
            BorderColor = Colors.Game.TurretLabels.Borders,
            BorderWidth = new Vector2(DAMAGE_COUNTER_BORDER_WIDTH),
            Position = new Vector2(center.X, Bounds.Y + DAMAGE_LABEL_OFFSET_Y)
        };
        _damageDealtLabel.CenterOrigin();

        _damageDealtCounter = new BorderedLabel(levelTextFont)
        {
            Text = "0",
            Scale = new Vector2(DamageLabelScale) * new Vector2(DamageCounterScaleX, 1),
            Color = Colors.Game.TurretLabels.Text,
            BorderColor = Colors.Game.TurretLabels.Borders,
            BorderWidth = new Vector2(DAMAGE_COUNTER_BORDER_WIDTH),
            Position = new Vector2(center.X, Bounds.Y + DAMAGE_COUNTER_OFFSET_Y)
        };
        _damageDealtCounter.CenterOrigin();
    }

    public void Initialize(Turret selectedTurret)
    {
        _turretDefinition.Text = $"{selectedTurret.Name} LVL ";
        _turretLevel.Text = $"{selectedTurret.Level + 1}";

        var panelMiddleX = _panelSprite.Width / 2;

        var turretDefinitionSizeX = _turretDefinition.MeasureText().X;

        var labelsLenght = turretDefinitionSizeX + _turretLevel.MeasureText().X;

        _turretDefinition.Position = _position + new Vector2(panelMiddleX - labelsLenght / 2, LABEL_OFFSET_Y);
        _turretLevel.Position = _turretDefinition.Position + new Vector2(turretDefinitionSizeX, -1);

        _upgradeButton.SetUpgradeCost(selectedTurret.GetUpgradePrice());
        _sellButton.SetSellCost(selectedTurret.GetSellPrice());

        UpdateDamageDealt(selectedTurret.DamageDealt);
    }

    public void Update(int userCoins, int damageDealt)
    {
        _upgradeButton.Update(userCoins);
        _sellButton.Update();

        UpdateDamageDealt(damageDealt);
    }

    private void UpdateDamageDealt(int damageDealt)
    {
        if (damageDealt == _damageDealt)
            return;

        _damageDealt = damageDealt;
        _damageDealtCounter.Text = damageDealt.ToString();
        _damageDealtCounter.CenterOrigin();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _panelSprite.Draw(spriteBatch, _position);
        _turretDefinition.Draw(spriteBatch);
        _turretLevel.Draw(spriteBatch);

        _damageDealtLabel.Draw(spriteBatch);
        _damageDealtCounter.Draw(spriteBatch);

        _upgradeButton.Draw(spriteBatch);
        _sellButton.Draw(spriteBatch);
    }
}