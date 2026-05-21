// using Microsoft.Xna.Framework;
// using MonoGameLibrary.Graphics.Labels;
// using MonoGameLibrary.Graphics.Sprites;
// using RKD_TD.Models.UI;
//
// namespace RKD_TD.Scenes.Gaming.Turrets.Actions;
//
// internal sealed class TurretUpgradeButton : Button
// {
//     private readonly Label _textLabel;
//     private readonly Label _priceLabel;
//
//     private int _upgradeCost;
//     
//     private bool CanUpgrade => _upgradeCost > 0;
//     
//     public TurretUpgradeButton(
//         Vector2 position, 
//         Vector2 origin, 
//         Sprite sprite, 
//         Color idleColor, 
//         Color hoveredColor,
//         Vector2 scale) 
//         : base(position, origin, sprite, idleColor, hoveredColor, scale, layerDepth: 1f)
//     {
//     }
//
//     public void SetUpgradeCost(int cost)
//     {
//         _upgradeCost = cost;
//
//         if (CanUpgrade)
//         {
//             _textLabel.Text = "UPGRADE";
//             _priceLabel.Text = _upgradeCost.ToString();
//         }
//         else
//         {
//             _textLabel.Text = "MAX LVL";
//             _priceLabel.Text = "";
//         }
//         
//     };
//
//     public void Update(int userCoins)
//     {
//         base.Update();
//     }
// }