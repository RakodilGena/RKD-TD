using Microsoft.Xna.Framework;

namespace RKD_TD.Assets;

internal static class Colors
{
    public static class Game
    {
        public static readonly Color TurretRadius = new(0, 0, 0, alpha: 80);

        public static readonly Color[] TurretLevels =
        [
            new(174, 192, 192),
            new(66, 153, 225),
            new(236, 175, 42)
            // new(88, 152, 188),
            // new(255, 172, 79),
            // new(200, 0, 0)
        ];

        public static readonly Color[] TurretPurchaseButtonPrices =
        [
            Color.Goldenrod,
            Color.DarkRed
        ];
    }
}