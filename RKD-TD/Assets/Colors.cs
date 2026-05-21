using Microsoft.Xna.Framework;

namespace RKD_TD.Assets;

internal static class Colors
{
    public static class Buttons
    {
        public static readonly Color
            Text = Color.White,
            TextBorders = Color.Black;


        public static readonly Color
            Idle = Color.White,
            Hovered = Color.DarkGray;
    }

    public static class MainTitle
    {
        public static readonly Color
            TitleColor = Color.White,
            TitleBorders = Color.Black;
    }

    public static class MapSelection
    {
        public static readonly Color
            TitleColor = Color.White,
            TitleBorders = Color.Black;
    }

    public static class Game
    {
        public static readonly Color TurretRadius = new(0, 0, 0, alpha: 80);

        public static readonly Color[] TurretLevels =
        [
            new(174, 192, 192),
            new(66, 153, 225),
            new(236, 175, 42)
        ];

        public static class TurretPrices
        {
            public static readonly Color
                Affordable = Color.Goldenrod,
                Unaffordable = Color.DarkRed,
                Borders = Color.Black;
        }

        public static class EndingTitle
        {
            public static readonly Color
                Victory = Color.Goldenrod,
                Defeat = Color.DarkRed,
                Borders = Color.Black;
        }

        public static class HealthBar
        {
            public static readonly Color
                Borders = Color.Black,
                Background = Color.Red,
                Filler = Color.LawnGreen;
        }

        public static readonly Color EnemyDying = Color.DarkGray;

        public static class Portals
        {
            public static readonly Color
                Starting = Color.LawnGreen,
                Ending = Color.Red;
        }

        public static class Labels
        {
            public static readonly Color
                Text = Color.White,
                TextBorders = Color.Black;
        }

        public static class CellHighlight
        {
            public static readonly Color
                Available = new(0, 255, 0, 60),
                NotAvailable = new(255, 0, 0, 60);
        }

        public static class GameClock
        {
            public static readonly Color
                Idle = Color.White,
                Hovered = Color.DarkGray,
                Toggled = Color.Orange,
                ToggledHovered = Color.DarkOrange;
        }
    }
}