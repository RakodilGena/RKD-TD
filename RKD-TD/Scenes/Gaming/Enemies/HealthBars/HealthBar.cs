using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Enemies.HealthBars;

internal sealed class HealthBar
{
    private readonly Vector2 _enemyOffset, _borders, _fillerInitialScale;
    private Vector2 _position;

    private readonly Sprite
        _bordersSprite,
        _backgroundSprite,
        _fillerSprite;


    private readonly int _maxHealth;

    public int CurrentHealth { get; private set; }

    public ICamera? Camera
    {
        get => _bordersSprite.Camera;
        set
        {
            _bordersSprite.Camera = value;
            _backgroundSprite.Camera = value;
            _fillerSprite.Camera = value;
        }
    }

    public HealthBar(
        HealthBarTemplate template)
    {
        _maxHealth = CurrentHealth = template.Health;

        _enemyOffset = template.EnemyOffset;
        _borders = template.Borders;

        _bordersSprite = new Sprite(template.BordersTexture)
        {
            Scale = template.BordersScale,
            Color = template.BordersColor
        };

        _backgroundSprite = new Sprite(template.BackgroundTexture)
        {
            Scale = template.FillerScale,
            Color = template.BackgroundColor
        };

        _fillerSprite = new Sprite(template.FillerTexture)
        {
            Scale = template.FillerScale,
            Color = template.FillerColor
        };
        _fillerInitialScale = template.FillerScale;
    }

    public void ReceiveDamage(int damage)
    {
        CurrentHealth -= damage;

        var healthPercent = (float)CurrentHealth / _maxHealth;
        _fillerSprite.Scale = new Vector2(_fillerInitialScale.X * healthPercent, _fillerInitialScale.Y);
    }

    public void SetPosition(Vector2 enemyPosition)
    {
        _position = enemyPosition + _enemyOffset;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (CurrentHealth == _maxHealth || CurrentHealth <= 0)
            return;

        var fillerPos = _position + _borders;

        _bordersSprite.Draw(spriteBatch, _position);
        _backgroundSprite.Draw(spriteBatch, fillerPos);
        _fillerSprite.Draw(spriteBatch, fillerPos);
    }
}