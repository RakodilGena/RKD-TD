using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

internal sealed class TurretPurchasePanel
{
    private readonly Vector2 _panelPosition;
    private readonly Sprite _panelSprite;

    private readonly Button _showButton;
    private readonly Button _hideButton;
    private bool _hidden;
    private readonly TurretPurchaseButton[] _turretPurchaseButtons;

    public event EventHandler<PendingTurretType>? TurretPicked;

    public TurretPurchasePanel(
        Vector2 position,
        TextureAtlas gameObjects,
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
            absoluteMargin: new Vector2(30, 18),
            scale * turretPurchaseButtonScale,
            buttonLayerDepth);
    }

    private void OnHideButtonClicked(object? sender, EventArgs args)
    {
        _hidden = true;
    }

    private void OnShowButtonClicked(object? sender, EventArgs args)
    {
        _hidden = false;
    }

    public void Update()
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
                turretPurchaseButton.Update();
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
        var hideButtonIdle = new Sprite(textureRegion);

        var hideButtonHovered = new Sprite(textureRegion)
        {
            Color = Color.DarkGray
        };

        return new Button(
            position,
            origin: Vector2.Zero,
            spriteIdle: hideButtonIdle,
            spriteHovered: hideButtonHovered,
            spritePressed: hideButtonHovered,
            scale,
            layerDepth: panelLayerDepth);
    }

    private static Button CreateShowButton(
        Vector2 position,
        TextureRegion textureRegion,
        Vector2 scale,
        float panelLayerDepth)
    {
        var showButtonIdle = new Sprite(textureRegion)
        {
            Effects = SpriteEffects.FlipHorizontally
        };

        var showButtonHovered = new Sprite(textureRegion)
        {
            Color = Color.DarkGray,
            Effects = SpriteEffects.FlipHorizontally
        };

        return new Button(
            position,
            origin: Vector2.Zero,
            spriteIdle: showButtonIdle,
            spriteHovered: showButtonHovered,
            spritePressed: showButtonHovered,
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
        Vector2 absoluteMargin,
        float scale,
        float buttonLayerDepth)
    {
        //var margin = new Vector2(260, 0) * scale;

        var buttonSize = new Vector2(240, 0) * scale;
        var horizontalMargin = new Vector2(absoluteMargin.X, 0) * scale;
        var verticalMargin = new Vector2(0, absoluteMargin.Y) * scale;

        var index = 0;

        var mgButton = CreateTurretPurchaseButton(
            textureAlias: Textures.Game.TURRET_ICON_MG_240,
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
        mgButton.Clicked += (_, _) => TurretPicked?.Invoke(this, PendingTurretType.MachineGun);

        var cannonButton = CreateTurretPurchaseButton(
            textureAlias: Textures.Game.TURRET_ICON_CANNON_240,
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
        cannonButton.Clicked += (_, _) => TurretPicked?.Invoke(this, PendingTurretType.Cannon);

        var shotgunButton = CreateTurretPurchaseButton(
            textureAlias: Textures.Game.TURRET_ICON_SHOTGUN_240,
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
        shotgunButton.Clicked += (_, _) => TurretPicked?.Invoke(this, PendingTurretType.Shotgun);

        var rocketButton = CreateTurretPurchaseButton(
            textureAlias: Textures.Game.TURRET_ICON_ROCKET_240,
            position + verticalMargin + buttonSize * index++ + horizontalMargin * index,
            gameObjects,
            scale,
            buttonLayerDepth);
        rocketButton.Clicked += (_, _) => TurretPicked?.Invoke(this, PendingTurretType.Rocket);


        return
        [
            mgButton,
            cannonButton,
            shotgunButton,
            rocketButton
        ];
    }

    private static TurretPurchaseButton CreateTurretPurchaseButton(
        string textureAlias,
        Vector2 position,
        TextureAtlas gameObjects,
        float scale,
        float buttonLayerDepth)
    {
        var idle = gameObjects.CreateSprite(textureAlias);

        var hovered = gameObjects.CreateSprite(textureAlias);
        hovered.Color = Color.DarkGray;

        return new TurretPurchaseButton(
            position,
            origin: Vector2.Zero,
            spriteIdle: idle,
            spriteHovered: hovered,
            spritePressed: hovered,
            scale: new Vector2(scale),
            buttonLayerDepth);
    }
}