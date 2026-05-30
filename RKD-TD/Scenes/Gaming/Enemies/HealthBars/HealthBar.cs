using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Visuals;

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
        HealthBarTemplate template,
        int waveIndex)
    {
        var currentHealth = template.Health *
                            MathF.Pow(template.HealthMultiplierPerWave, waveIndex)
                            + waveIndex * template.HealthIncreasePerWave;

        _maxHealth = CurrentHealth = (int)currentHealth;

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

    public int ReceiveDamage(int damage)
    {
        if (CurrentHealth <= 0)
            return 0;

        int damageDealt;
        float healthPercent;
        if (CurrentHealth <= damage)
        {
            damageDealt = CurrentHealth;

            CurrentHealth = 0;
            healthPercent = 0;
        }
        else
        {
            damageDealt = damage;

            CurrentHealth -= damage;
            healthPercent = (float)CurrentHealth / _maxHealth;
        }

        _fillerSprite.Scale = new Vector2(_fillerInitialScale.X * healthPercent, _fillerInitialScale.Y);

        return damageDealt;
    }

    public void SetPosition(Vector2 enemyPosition)
    {
        _position = enemyPosition + _enemyOffset;
    }

    public void Draw(SpriteBatch spriteBatch, EnemyState state)
    {
        if (CurrentHealth == _maxHealth || state is not EnemyState.Vulnerable)
            return;

        var fillerPos = _position + _borders;

        _bordersSprite.Draw(spriteBatch, _position);
        _backgroundSprite.Draw(spriteBatch, fillerPos);
        _fillerSprite.Draw(spriteBatch, fillerPos);
    }
}