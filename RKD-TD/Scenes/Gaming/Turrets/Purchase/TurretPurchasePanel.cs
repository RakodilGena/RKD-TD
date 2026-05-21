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
    private readonly Vector2 _panelPosition;
    private readonly Sprite _panelSprite;

    private readonly Button _showButton;
    private readonly Button _hideButton;
    private bool _hidden;
    private readonly TurretPurchaseButton[] _turretPurchaseButtons;

    public event EventHandler<TurretType>? TurretPicked;

    private readonly Rectangle _panelBounds;

    private static readonly Color ButtonIdleColor = Color.White;
    private static readonly Color ButtonHoveredColor = Color.DarkGray;

    public TurretPurchasePanel(
        Vector2 position,
        TextureAtlas gameObjects,
        PendingTurretStash pendingTurretStash,
        float scale,
        float panelLayerDepth,
        float buttonLayerDepth)
    {
        var showHideButtonTexture = gameObjects.GetRegion(Textures.Game.TURRET_PANEL_HIDE_BTN_125_250);

        var showHideBtnPosition = position + new Vector2(0, 10) * scale;
        var showHideBtnScale = new Vector2(scale, 0.92f * scale);

        _hideButton = CreateHideButton(
            showHideBtnPosition,
            showHideButtonTexture,
            showHideBtnScale,
            panelLayerDepth);
        _hideButton.Clicked += OnHideButtonClicked;

        _showButton = CreateShowButton(
            showHideBtnPosition,
            showHideButtonTexture,
            showHideBtnScale,
            panelLayerDepth);
        _showButton.Clicked += OnShowButtonClicked;

        _panelPosition = position + new Vector2(130, 0) * scale;
        _panelSprite = CreatePanelSprite(
            gameObjects,
            scale,
            panelLayerDepth);

        const float turretPurchaseButtonScale = 0.90f;
        _turretPurchaseButtons = CreateTurretPurchaseButtons(
            _panelPosition,
            gameObjects,
            pendingTurretStash,
            absoluteMargin: new Vector2(30, 18),
            scale * turretPurchaseButtonScale,
            buttonLayerDepth);

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
        Vector2 scale,
        float panelLayerDepth)
    {
        var hideButton = new Sprite(textureRegion);

        return new Button(
            position,
            origin: Vector2.Zero,
            hideButton,
            ButtonIdleColor,
            ButtonHoveredColor,
            scale,
            layerDepth: panelLayerDepth);
    }

    private static Button CreateShowButton(
        Vector2 position,
        TextureRegion textureRegion,
        Vector2 scale,
        float panelLayerDepth)
    {
        var showButton = new Sprite(textureRegion)
        {
            Effects = SpriteEffects.FlipHorizontally
        };

        return new Button(
            position,
            origin: Vector2.Zero,
            showButton,
            ButtonIdleColor,
            ButtonHoveredColor,
            scale,
            layerDepth: panelLayerDepth);
    }

    private static Sprite CreatePanelSprite(
        TextureAtlas gameObjects,
        float scale,
        float panelLayerDepth)
    {
        var panelSprite = gameObjects.CreateSprite(Textures.Game.TURRET_PANEL_1000_250);
        panelSprite.LayerDepth = panelLayerDepth;
        panelSprite.Scale = new Vector2(scale);

        return panelSprite;
    }


    private TurretPurchaseButton[] CreateTurretPurchaseButtons(
        Vector2 position,
        TextureAtlas gameObjects,
        PendingTurretStash pendingTurretStash,
        Vector2 absoluteMargin,
        float scale,
        float buttonLayerDepth)
    {
        var buttonSize = new Vector2(240, 0) * scale;
        var horizontalMargin = new Vector2(absoluteMargin.X, 0) * scale;
        var verticalMargin = new Vector2(0, absoluteMargin.Y) * scale;
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
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
        mgButton.Clicked += (_, _) => TurretPicked?.Invoke(this, TurretType.MachineGun);

        var shotgunTemplate = pendingTurretStash.GetPendingTurret(TurretType.Shotgun);
        var shotgunButton = CreateTurretPurchaseButton(
            shotgunTemplate.Name,
            shotgunTemplate.Price,
            nameFont,
            priceFont,
            textureAlias: Textures.Game.TURRET_ICON_SHOTGUN_240,
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
        shotgunButton.Clicked += (_, _) => TurretPicked?.Invoke(this, TurretType.Shotgun);

        var cannonTemplate = pendingTurretStash.GetPendingTurret(TurretType.Cannon);
        var cannonButton = CreateTurretPurchaseButton(
            cannonTemplate.Name,
            cannonTemplate.Price,
            nameFont,
            priceFont,
            textureAlias: Textures.Game.TURRET_ICON_CANNON_240,
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
        cannonButton.Clicked += (_, _) => TurretPicked?.Invoke(this, TurretType.Cannon);

        var missileTemplate = pendingTurretStash.GetPendingTurret(TurretType.Missile);
        var missileButton = CreateTurretPurchaseButton(
            missileTemplate.Name,
            missileTemplate.Price,
            nameFont,
            priceFont,
            textureAlias: Textures.Game.TURRET_ICON_MISSILE_240,
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
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
        TextureAtlas gameObjects,
        float scale,
        float buttonLayerDepth)
    {
        var sprite = gameObjects.CreateSprite(textureAlias);

        return new TurretPurchaseButton(
            name,
            price,
            nameFont,
            priceFont,
            position,
            origin: Vector2.Zero,
            sprite,
            ButtonIdleColor,
            ButtonHoveredColor,
            scale: new Vector2(scale),
            buttonLayerDepth);
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