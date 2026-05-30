using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;
using RKD_TD.Models.UI;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class TurretPurchasePanel
{
    private const int
        PANEL_WIDTH = 600,
        PANEL_HEIGHT = 150,
        PANEL_OFFSET_X = 80,
        SHOW_HIDE_BUTTON_WIDTH = 75,
        SHOW_HIDE_BUTTON_HEIGHT = 138,
        SHOW_HIDE_BUTTON_OFFSET_Y = 6,
        PURCHASE_BUTTON_SIZE = 130,
        PURCHASE_BUTTON_MARGIN_X = 16,
        PURCHASE_BUTTON_MARGIN_Y = 11;


    private readonly Vector2 _panelPosition;
    private readonly Sprite _panelSprite;

    private readonly Button _showButton;
    private readonly Button _hideButton;
    private bool _hidden;
    private readonly TurretPurchaseButton[] _turretPurchaseButtons;

    public event EventHandler<TurretType>? TurretPicked;

    private readonly Rectangle _panelBounds;


    public TurretPurchasePanel(
        Vector2 position,
        TextureAtlas gameObjects,
        PendingTurretStash pendingTurretStash)
    {
        var showHideButtonTexture = gameObjects.GetRegion(Textures.Game.TURRET_PANEL_HIDE_BTN_125_250);

        var showHideBtnPosition = position + new Vector2(0, SHOW_HIDE_BUTTON_OFFSET_Y);

        var showHideBtnScale = new Vector2(
            (float)SHOW_HIDE_BUTTON_WIDTH / showHideButtonTexture.Width,
            (float)SHOW_HIDE_BUTTON_HEIGHT / showHideButtonTexture.Height);

        _hideButton = CreateHideButton(
            showHideBtnPosition,
            showHideButtonTexture,
            showHideBtnScale);
        _hideButton.Clicked += OnHideButtonClicked;

        _showButton = CreateShowButton(
            showHideBtnPosition,
            showHideButtonTexture,
            showHideBtnScale);
        _showButton.Clicked += OnShowButtonClicked;

        _panelPosition = position + new Vector2(PANEL_OFFSET_X, 0);
        _panelSprite = CreatePanelSprite(gameObjects);

        _turretPurchaseButtons = CreateTurretPurchaseButtons(
            _panelPosition,
            gameObjects,
            pendingTurretStash);

        _panelBounds = new Rectangle(
            (int)_panelPosition.X,
            (int)_panelPosition.Y,
            (int)_panelSprite.Width,
            (int)_panelSprite.Height);
    }

    private void OnHideButtonClicked(object? sender, EventArgs args)
    {
        _hidden = true;
    }

    private void OnShowButtonClicked(object? sender, EventArgs args)
    {
        _hidden = false;
    }

    public void Update(int userCoins)
    {
        if (_hidden)
        {
            _showButton.Update();
        }
        else
        {
            _hideButton.Update();
            foreach (var turretPurchaseButton in _turretPurchaseButtons)
            {
                turretPurchaseButton.Update(userCoins);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_hidden)
        {
            _showButton.Draw(spriteBatch);
        }
        else
        {
            _hideButton.Draw(spriteBatch);
            _panelSprite.Draw(spriteBatch, _panelPosition);
            foreach (var turretPurchaseButton in _turretPurchaseButtons)
            {
                turretPurchaseButton.Draw(spriteBatch);
            }
        }
    }

    private static Button CreateHideButton(
        Vector2 position,
        TextureRegion textureRegion,
        Vector2 scale)
    {
        var hideButton = new Sprite(textureRegion);

        return new Button(
            position,
            origin: Vector2.Zero,
            hideButton,
            Colors.Buttons.Idle,
            Colors.Buttons.Hovered,
            scale,
            layerDepth: 0);
    }

    private static Button CreateShowButton(
        Vector2 position,
        TextureRegion textureRegion,
        Vector2 scale)
    {
        var showButton = new Sprite(textureRegion)
        {
            Effects = SpriteEffects.FlipHorizontally
        };

        return new Button(
            position,
            origin: Vector2.Zero,
            showButton,
            Colors.Buttons.Idle,
            Colors.Buttons.Hovered,
            scale,
            layerDepth: 0);
    }

    private static Sprite CreatePanelSprite(TextureAtlas gameObjects)
    {
        var panelSprite = gameObjects.CreateSprite(Textures.Game.TURRET_PANEL_1000_250);
        panelSprite.Scale = new Vector2(
            PANEL_WIDTH / panelSprite.Width,
            PANEL_HEIGHT / panelSprite.Height);

        return panelSprite;
    }


    private TurretPurchaseButton[] CreateTurretPurchaseButtons(
        Vector2 position,
        TextureAtlas gameObjects,
        PendingTurretStash pendingTurretStash)
    {
        var absoluteMargin = new Vector2(PURCHASE_BUTTON_MARGIN_X, PURCHASE_BUTTON_MARGIN_Y);

        var buttonSizeX = new Vector2(PURCHASE_BUTTON_SIZE, 0);

        var horizontalMargin = new Vector2(absoluteMargin.X, 0);
        var verticalMargin = new Vector2(0, absoluteMargin.Y);

        var nameFont = GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_PURCHASE_BTN_TEXT);
        var priceFont = GlobalAssets.FontAtlas.GetFont(Fonts.TURRET_PURCHASE_BTN_PRICE_TEXT);

        var index = 0;

        var mgTemplate = pendingTurretStash.GetPendingTurret(TurretType.MachineGun);
        var mgButton = CreateTurretPurchaseButton(
            mgTemplate.Name,
            mgTemplate.Price,
            nameFont,
            priceFont,
            textureAlias: Textures.Game.TURRET_ICON_MG_240,
            position + verticalMargin + buttonSizeX * index++ + horizontalMargin * index,
            gameObjects);
        mgButton.Clicked += (_, _) => TurretPicked?.Invoke(this, TurretType.MachineGun);

        var shotgunTemplate = pendingTurretStash.GetPendingTurret(TurretType.Shotgun);
        var shotgunButton = CreateTurretPurchaseButton(
            shotgunTemplate.Name,
            shotgunTemplate.Price,
            nameFont,
            priceFont,
            textureAlias: Textures.Game.TURRET_ICON_SHOTGUN_240,
            position + verticalMargin + buttonSizeX * index++ + horizontalMargin * index,
            gameObjects);
        shotgunButton.Clicked += (_, _) => TurretPicked?.Invoke(this, TurretType.Shotgun);

        var cannonTemplate = pendingTurretStash.GetPendingTurret(TurretType.Cannon);
        var cannonButton = CreateTurretPurchaseButton(
            cannonTemplate.Name,
            cannonTemplate.Price,
            nameFont,
            priceFont,
            textureAlias: Textures.Game.TURRET_ICON_CANNON_240,
            position + verticalMargin + buttonSizeX * index++ + horizontalMargin * index,
            gameObjects);
        cannonButton.Clicked += (_, _) => TurretPicked?.Invoke(this, TurretType.Cannon);

        var missileTemplate = pendingTurretStash.GetPendingTurret(TurretType.Missile);
        var missileButton = CreateTurretPurchaseButton(
            missileTemplate.Name,
            missileTemplate.Price,
            nameFont,
            priceFont,
            textureAlias: Textures.Game.TURRET_ICON_MISSILE_240,
            position + verticalMargin + buttonSizeX * index++ + horizontalMargin * index,
            gameObjects);
        missileButton.Clicked += (_, _) => TurretPicked?.Invoke(this, TurretType.Missile);


        return
        [
            mgButton,
            cannonButton,
            shotgunButton,
            missileButton
        ];
    }

    private static TurretPurchaseButton CreateTurretPurchaseButton(
        string name,
        int price,
        SpriteFont nameFont,
        SpriteFont priceFont,
        string textureAlias,
        Vector2 position,
        TextureAtlas gameObjects)
    {
        var sprite = gameObjects.CreateSprite(textureAlias);

        var scale = new Vector2(
            PURCHASE_BUTTON_SIZE / sprite.Width,
            PURCHASE_BUTTON_SIZE / sprite.Height);

        return new TurretPurchaseButton(
            name,
            price,
            nameFont,
            priceFont,
            position,
            origin: Vector2.Zero,
            sprite,
            Colors.Buttons.Idle,
            Colors.Buttons.Hovered,
            scale,
            layerDepth: 0);
    }

    public Rectangle[] GetPanelBounds()
    {
        if (_hidden)
        {
            return [_showButton.Bounds];
        }

        return [_hideButton.Bounds, _panelBounds];
    }
}