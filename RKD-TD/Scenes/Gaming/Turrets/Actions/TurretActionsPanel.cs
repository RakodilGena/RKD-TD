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
        SIZE_Y = 400,
        
        LABEL_OFFSET_Y = 20,
        LABEL_BORDER_WIDTH = 3,
        LVL_LABEL_OFFSET_X = 10;

    private readonly Vector2 _position;
    private readonly Sprite _panelSprite;
    private readonly Label _turretDefinition, _turretLevel;
    
    public Rectangle Bounds { get; }
    
    //private readonly TurretUpgradeButton _upgradeButton;
    //private readonly TurretSellButton _sellButton;

    public TurretActionsPanel(
        Vector2 position,
        TextureAtlas textures)
    {
        _position = position;
        
        _panelSprite = textures.CreateSprite(Textures.Game.PANEL);
        _panelSprite.Scale = new Vector2(SIZE_X /_panelSprite.Width, SIZE_Y / _panelSprite.Height);
        


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
    }

    public void Initialize(Turret selectedTurret)
    {
        _turretDefinition.Text = $"{selectedTurret.Name}  LVL";
        _turretLevel.Text = $"{selectedTurret.GetLevel()+1}";
        
        var panelMiddleX = _panelSprite.Width / 2;

        var turretDefinitionSizeX = _turretDefinition.MeasureText().X;

        var labelsLenght = turretDefinitionSizeX + LVL_LABEL_OFFSET_X + _turretLevel.MeasureText().X;

        _turretDefinition.Position = _position + new Vector2(panelMiddleX - labelsLenght / 2, LABEL_OFFSET_Y);
        _turretLevel.Position = _turretDefinition.Position + new Vector2(turretDefinitionSizeX + LVL_LABEL_OFFSET_X, 0);

        //todo: reinit buttons
    }

    public void Update(int userCoins)
    {
        //todo update buttons
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _panelSprite.Draw(spriteBatch, _position);
        _turretDefinition.Draw(spriteBatch);
        _turretLevel.Draw(spriteBatch);
    }
}