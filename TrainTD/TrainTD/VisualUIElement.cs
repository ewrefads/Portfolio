using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrainTD
{
    /// <summary>
    /// Klasse for UI elementer der ikke har noget funktionalitet
    /// </summary>
    public class VisualUIElement : UIElement
    {
        public VisualUIElement(Player activePlayer, Texture2D[] sprites, Color activeColor, Color hoverColor, Color clickColor, Color inActiveColor, Vector2 position, float animationTime) : base(activePlayer, sprites, activeColor, hoverColor, clickColor, inActiveColor, position, animationTime)
        {
        }
    }
}
