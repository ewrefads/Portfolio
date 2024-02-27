using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrainTD
{
    /// <summary>
    /// Knap til at afslutte den nuværende fase
    /// </summary>
    public class EndPhaseButton:UIElement
    {
        public EndPhaseButton(Player activePlayer, Texture2D[] sprites, Color activeColor, Color hoverColor, Color clickColor, Color inActiveColor, Vector2 position, float animationTime) : base(activePlayer, sprites, activeColor, hoverColor, clickColor, inActiveColor, position, animationTime)
        {
            active = true;
        }

        /// <summary>
        /// Kalder EndPhase i gameWorld
        /// </summary>
        protected override void OnCLick()
        {
            base.OnCLick();
            GameWorld.EndPhase();
            
        }
    }
}
