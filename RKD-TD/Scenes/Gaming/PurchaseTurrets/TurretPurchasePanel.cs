using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

public sealed class TurretPurchasePanel
{
    private readonly Vector2 _panelPosition;
    private readonly Sprite _panelSprite;

    private readonly Button _showButton;
    private readonly Button _hideButton;
    private bool _hidden;
    //private readonly TurretPurchaseButton[] _turretPurchaseButtons;
    
    public TurretPurchasePanel(
        Vector2 position,
        TextureAtlas gameObjects,
        float scale,
        float panelLayerDepth,
        float buttonLayerDepth)
    {
        var showHideButtonTexture = gameObjects.GetRegion(Textures.Game.TURRET_PANEL_HIDE_BTN_125_250);

        var showHideBtnPosition = position + new Vector2(0,10) * scale;
        var showHideBtnScale = new Vector2(scale,  0.92f * scale);
        
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
        _panelSprite = gameObjects.CreateSprite(Textures.Game.TURRET_PANEL_1000_250);
        _panelSprite.LayerDepth = panelLayerDepth;
        _panelSprite.Scale = new Vector2(scale);
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
            //todo: update buttons
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
            //todo: draw buttons
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
}